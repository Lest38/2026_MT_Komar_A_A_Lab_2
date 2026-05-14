using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Models;

namespace Services;

#nullable enable
public class PipelineRunner(
    ILogger<PipelineRunner> logger,
    ConfigurationService configService,
    GitService gitService,
    DotNetService dotNetService)
{
    private const string DotnetCommand = "dotnet";
    private const string GitCommand = "git";
    private const int MaxOutputLength = 500;

    private static readonly Action<ILogger, Exception?> LogPipelineExecutionStarted =
        LoggerMessage.Define(
            LogLevel.Information,
            new EventId(6001, nameof(RunPipelineAsync)),
            "=== Pipeline Execution Started ===");

    private static readonly Action<ILogger, int, Exception?> LogPipelineLoaded =
        LoggerMessage.Define<int>(
            LogLevel.Information,
            new EventId(6002, nameof(RunPipelineAsync)),
            "Loaded pipeline with {StageCount} stages");

    private static readonly Action<ILogger, string, Exception?> LogStageSuccess =
    LoggerMessage.Define<string>(
        LogLevel.Information,
        new EventId(6003, nameof(HandleSuccessfulStage)),
        "[SUCCESS] Stage '{StageName}' finished successfully");

    private static readonly Action<ILogger, string, int, Exception?> LogStageError =
        LoggerMessage.Define<string, int>(
            LogLevel.Error,
            new EventId(6004, nameof(HandleFailedStage)),
            "[ERROR] Stage '{StageName}' failed with ExitCode {ExitCode}");

    private static readonly Action<ILogger, string, int, Exception?> LogStageTimeout =
        LoggerMessage.Define<string, int>(
            LogLevel.Error,
            new EventId(6005, nameof(HandleFailedStage)),
            "Stage '{StageName}' timed out after {TimeoutSeconds} seconds");

    private static readonly Action<ILogger, string, Exception?> LogStoppingPipeline =
        LoggerMessage.Define<string>(
            LogLevel.Warning,
            new EventId(6006, nameof(HandleFailedStage)),
            "Stopping pipeline due to StopOnFailure flag on stage '{StageName}'");

    private static readonly Action<ILogger, Exception?> LogContinuingPipeline =
        LoggerMessage.Define(
            LogLevel.Information,
            new EventId(6007, nameof(HandleFailedStage)),
            "Continuing pipeline despite failure (StopOnFailure=false)");

    private static readonly Action<ILogger, string, Exception?> LogSavedArtifact =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(6008, nameof(SaveArtifact)),
            "Saved artifact for stage '{StageName}'");

    private static readonly Action<ILogger, string, Exception?> LogFailedToSaveArtifact =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(6009, nameof(SaveArtifact)),
            "Failed to save artifact for stage '{StageName}'");

    private static readonly Action<ILogger, string, string, Exception?> LogSetEnvironmentVariable =
        LoggerMessage.Define<string, string>(
            LogLevel.Debug,
            new EventId(6010, nameof(SetEnvironmentVariables)),
            "Set environment variable: {EnvKey}={EnvValue}");

    private static readonly Action<ILogger, string?, Exception?> LogExceptionExecutingStage =
    LoggerMessage.Define<string?>(
        LogLevel.Error,
        new EventId(6011, nameof(ExecuteStageAsync)),
        "Exception executing stage '{StageName}'");

    private static readonly Action<ILogger, string, Exception?> LogStageExecutionDetails =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(6012, nameof(LogStageExecution)),
            "{StageDetails}");

    private static readonly Action<ILogger, string, Exception?> LogPipelineSummaryWarning =
        LoggerMessage.Define<string>(
            LogLevel.Warning,
            new EventId(6013, nameof(LogPipelineSummary)),
            "=== Pipeline Execution Summary ===\n{Summary}");

    private static readonly Action<ILogger, string, Exception?> LogPipelineSummaryInfo =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(6014, nameof(LogPipelineSummary)),
            "=== Pipeline Execution Summary ===\n{Summary}");

    private readonly ILogger<PipelineRunner> logger = logger;
    private readonly ConfigurationService configService = configService;
    private readonly GitService gitService = gitService;
    private readonly DotNetService dotNetService = dotNetService;
    private readonly Dictionary<string, DateTime> stageTimings =
        [];

    public async Task<int> RunPipelineAsync(string configPath, string targetDir)
    {
        var stopwatch = Stopwatch.StartNew();
        LogPipelineExecutionStarted(this.logger, null);

        var config = this.configService.LoadConfiguration(configPath);
        LogPipelineLoaded(this.logger, config.Pipeline.Count, null);

        var stats = new PipelineStats
        {
            TotalStages = config.Pipeline.Count,
        };

        foreach (var stage in config.Pipeline)
        {
            var shouldContinue = await this.ExecuteStageWithStats(stage, targetDir, stats).ConfigureAwait(false);
            if (!shouldContinue)
            {
                break;
            }
        }

        this.LogPipelineSummary(stopwatch.Elapsed, config.Pipeline.Count, stats);
        return stats.FailedStages > 0 ? -1 : 0;
    }

    private static ProcessResult CreateErrorResult(PipelineItem stage, string errorMessage)
    {
        return new ProcessResult
        {
            Command = stage.Command,
            Arguments = stage.Args,
            ExitCode = -1,
            Errors = errorMessage,
            StartTime = DateTime.Now,
            EndTime = DateTime.Now,
            DurationMs = 0,
        };
    }

    private async Task<bool> ExecuteStageWithStats(PipelineItem stage, string targetDir, PipelineStats stats)
    {
        stats.StageNumber++;

        var stageStopwatch = Stopwatch.StartNew();
        var result = await this.ExecuteStageAsync(stage, targetDir).ConfigureAwait(false);
        stageStopwatch.Stop();

        this.stageTimings[stage.Name] = DateTime.Now;

        this.LogStageExecution(
            stage,
            stats.StageNumber,
            stats.TotalStages,
            result,
            stageStopwatch.ElapsedMilliseconds);

        if (result.IsSuccess)
        {
            stats.SuccessfulStages++;
            await this.HandleSuccessfulStage(stage.Name, result.Output).ConfigureAwait(false);
            return true;
        }

        stats.FailedStages++;
        return this.HandleFailedStage(stage, result);
    }

    private void LogStageExecution(
        PipelineItem stage,
        int stageNumber,
        int totalStages,
        ProcessResult result,
        long durationMs)
    {
        string status = result.IsSuccess ? "SUCCESS" : "FAILED";
        string outputInfo = !string.IsNullOrEmpty(result.Output) && result.Output.Length < MaxOutputLength
            ? $"\nOutput: {result.Output.Trim()}"
            : string.Empty;
        string errorsInfo = !string.IsNullOrEmpty(result.Errors)
            ? $"\nErrors: {result.Errors.Trim()}"
            : string.Empty;

        string stageDetails =
            $"\n[{stageNumber}/{totalStages}] Stage: {stage.Name}\n" +
            $"Command: {stage.Command} {stage.Args}\n" +
            $"Stop on failure: {stage.StopOnFailure}\n" +
            $"Status: {status} (ExitCode: {result.ExitCode}, Duration: {durationMs}ms){outputInfo}{errorsInfo}";

        LogStageExecutionDetails(this.logger, stageDetails, null);
    }

    private async Task HandleSuccessfulStage(string stageName, string output)
    {
        LogStageSuccess(this.logger, stageName, null);

        if (!string.IsNullOrEmpty(output))
        {
            await this.SaveArtifact(stageName, output).ConfigureAwait(false);
        }
    }

    private bool HandleFailedStage(PipelineItem stage, ProcessResult result)
    {
        LogStageError(this.logger, stage.Name, result.ExitCode, null);

        if (result.IsTimeout)
        {
            LogStageTimeout(this.logger, stage.Name, stage.TimeoutSeconds, null);
        }

        if (stage.StopOnFailure)
        {
            LogStoppingPipeline(this.logger, stage.Name, null);
            return false;
        }

        LogContinuingPipeline(this.logger, null);
        return true;
    }

    private void LogPipelineSummary(TimeSpan elapsedTime, int totalStages, PipelineStats stats)
    {
        string summary = $"Total execution time: {elapsedTime.TotalMilliseconds}ms ({elapsedTime:ss\\:ff} seconds)\n" +
                         $"Stages: Total={totalStages}, Successful={stats.SuccessfulStages}, Failed={stats.FailedStages}";

        if (stats.FailedStages > 0)
        {
            LogPipelineSummaryWarning(this.logger, summary, null);
        }
        else
        {
            LogPipelineSummaryInfo(this.logger, summary, null);
        }
    }

    private async Task<ProcessResult> ExecuteStageAsync(PipelineItem stage, string targetDir)
    {
        try
        {
            string workingDir = stage.WorkingDirectory ?? targetDir;
            this.SetEnvironmentVariables(stage);

            string command = stage.Command.ToLower(CultureInfo.CurrentCulture);
            string args = stage.Args;

            if (command == GitCommand)
            {
                return await this.ExecuteGitCommandAsync(stage, workingDir, args).ConfigureAwait(false);
            }

            if (command == DotnetCommand)
            {
                return await this.ExecuteDotNetCommandAsync(stage, workingDir, args).ConfigureAwait(false);
            }

            return await ProcessRunner.RunCommandAsync(
                stage.Command,
                stage.Args,
                workingDir,
                waitForExit: true,
                timeoutSeconds: stage.TimeoutSeconds).ConfigureAwait(false);
        }
        catch (ArgumentException ex)
        {
            LogExceptionExecutingStage(this.logger, stage.Name, ex);
            return CreateErrorResult(stage, ex.Message);
        }
    }

    private async Task<ProcessResult> ExecuteGitCommandAsync(
        PipelineItem stage,
        string workingDir,
        string args)
    {
        if (args.StartsWith("clone", StringComparison.OrdinalIgnoreCase))
        {
            string repoUrl = args.Replace("clone", string.Empty, StringComparison.OrdinalIgnoreCase).Trim();
            string repoUrlPart = repoUrl.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0];

            if (Uri.TryCreate(repoUrlPart, UriKind.Absolute, out var uri))
            {
                return await this.gitService.CloneAsync(workingDir, uri).ConfigureAwait(false);
            }
            else
            {
                return CreateErrorResult(stage, $"Invalid repository URL: {repoUrlPart}");
            }
        }

        if (args.Contains("pull", StringComparison.OrdinalIgnoreCase))
        {
            return await this.gitService.PullAsync(workingDir).ConfigureAwait(false);
        }

        if (args.Contains("branch", StringComparison.OrdinalIgnoreCase))
        {
            return await GitService.GetCurrentBranchAsync(workingDir).ConfigureAwait(false);
        }

        if (args.Contains("status", StringComparison.OrdinalIgnoreCase))
        {
            return await GitService.GetStatusAsync(workingDir).ConfigureAwait(false);
        }

        return await ProcessRunner.RunCommandAsync(
            stage.Command,
            stage.Args,
            workingDir,
            waitForExit: true,
            timeoutSeconds: stage.TimeoutSeconds).ConfigureAwait(false);
    }

    private async Task<ProcessResult> ExecuteDotNetCommandAsync(
        PipelineItem stage,
        string workingDir,
        string args)
    {
        if (args.Contains("clean", StringComparison.OrdinalIgnoreCase))
        {
            return await this.dotNetService.CleanAsync(workingDir).ConfigureAwait(false);
        }

        if (args.Contains("restore", StringComparison.OrdinalIgnoreCase))
        {
            return await this.dotNetService.RestoreAsync(workingDir).ConfigureAwait(false);
        }

        if (args.Contains("build", StringComparison.OrdinalIgnoreCase))
        {
            return await this.dotNetService.BuildAsync(workingDir).ConfigureAwait(false);
        }

        if (args.Contains("test", StringComparison.OrdinalIgnoreCase))
        {
            return await this.dotNetService.TestAsync(workingDir).ConfigureAwait(false);
        }

        if (args.Contains("run", StringComparison.OrdinalIgnoreCase))
        {
            return await this.dotNetService.RunAsync(workingDir, waitForExit: true).ConfigureAwait(false);
        }

        return await ProcessRunner.RunCommandAsync(
            stage.Command,
            stage.Args,
            workingDir,
            waitForExit: true,
            timeoutSeconds: stage.TimeoutSeconds).ConfigureAwait(false);
    }

    private void SetEnvironmentVariables(PipelineItem stage)
    {
        if (stage.Environment == null || stage.Environment.Count == 0)
        {
            return;
        }

        foreach (var env in stage.Environment)
        {
            Environment.SetEnvironmentVariable(env.Key, env.Value);
            LogSetEnvironmentVariable(this.logger, env.Key, env.Value, null);
        }
    }

    private async Task SaveArtifact(string stageName, string output)
    {
        try
        {
            var artifactsDir = Path.Combine(Directory.GetCurrentDirectory(), "artifacts");
            Directory.CreateDirectory(artifactsDir);

            var artifactFile = Path.Combine(artifactsDir, $"{stageName}_{DateTime.Now:yyyyMMdd_HHmmss}.log");
            await File.WriteAllTextAsync(artifactFile, output).ConfigureAwait(false);

            LogSavedArtifact(this.logger, stageName, null);
        }
        catch (ArgumentException ex)
        {
            LogFailedToSaveArtifact(this.logger, stageName, ex);
        }
    }
}