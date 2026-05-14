using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Models;

namespace Services;

#nullable enable
public class DotNetService(
    ProjectResolver projectResolver,
    ILogger<DotNetService> logger)
{
    private static readonly EventId EventIdCleaning = new
        (1001, nameof(CleanAsync));

    private static readonly EventId EventIdRestoring = new
        (1002, nameof(RestoreAsync));

    private static readonly EventId EventIdBuilding = new
        (1003, nameof(BuildAsync));

    private static readonly EventId EventIdRunningTests = new
        (1004, nameof(TestAsync));

    private static readonly EventId EventIdStartingApp = new
        (1005, nameof(RunAsync));

    private static readonly EventId EventIdStartingProject = new
        (1006, nameof(RunProjectAsync));

    private static readonly EventId EventIdRunningDotNet = new
        (1007, "RunCommandAsync");

    private static readonly Action<ILogger, string, Exception?> LogCleaningSolution =
        LoggerMessage.Define<string>(
            logLevel: LogLevel.Information,
            eventId: EventIdCleaning,
            formatString: "Cleaning solution in {TargetDir}");

    private static readonly Action<ILogger, string, Exception?> LogRestoringPackages =
        LoggerMessage.Define<string>(
            logLevel: LogLevel.Information,
            eventId: EventIdRestoring,
            formatString: "Restoring packages in {TargetDir}");

    private static readonly Action<ILogger, string, Exception?> LogBuildingSolution =
        LoggerMessage.Define<string>(
            logLevel: LogLevel.Information,
            eventId: EventIdBuilding,
            formatString: "Building solution in {TargetDir}");

    private static readonly Action<ILogger, string, Exception?> LogRunningTests =
        LoggerMessage.Define<string>(
            logLevel: LogLevel.Information,
            eventId: EventIdRunningTests,
            formatString: "Running tests in {TargetDir}");

    private static readonly Action<ILogger, string, Exception?> LogStartingApplication =
        LoggerMessage.Define<string>(
            logLevel: LogLevel.Information,
            eventId: EventIdStartingApp,
            formatString: "Starting application in {TargetDir}");

    private static readonly Action<ILogger, string, string, Exception?> LogStartingProject =
        LoggerMessage.Define<string, string>(
            logLevel: LogLevel.Information,
            eventId: EventIdStartingProject,
            formatString: "Starting project {ProjectName} in {TargetDir}");

    private static readonly Action<ILogger, string, string, Exception?> LogRunningDotNet =
        LoggerMessage.Define<string, string>(
            logLevel: LogLevel.Information,
            eventId: EventIdRunningDotNet,
            formatString: "Running: dotnet {Args} in {TargetDir}");

    private readonly ProjectResolver projectResolver = projectResolver;
    private readonly ILogger<DotNetService> logger = logger;

    public async Task<ProcessResult> RunCommandAsync(string targetDir, string args, bool waitForExit = true)
    {
        args = this.projectResolver.ResolveProjectIfNeeded(targetDir, args);

        LogRunningDotNet(this.logger, args, targetDir, null);
        return await ProcessRunner.RunCommandAsync(
            "dotnet",
            args,
            targetDir,
            waitForExit,
            timeoutSeconds: TimeoutDefaults.Default)
            .ConfigureAwait(false);
    }

    public async Task<ProcessResult> CleanAsync(string targetDir)
    {
        LogCleaningSolution(this.logger, targetDir, null);
        return await this.RunCommandAsync(targetDir, "clean").ConfigureAwait(false);
    }

    public async Task<ProcessResult> RestoreAsync(string targetDir)
    {
        LogRestoringPackages(this.logger, targetDir, null);
        return await this.RunCommandAsync(targetDir, "restore").ConfigureAwait(false);
    }

    public async Task<ProcessResult> BuildAsync(string targetDir)
    {
        LogBuildingSolution(this.logger, targetDir, null);
        return await this.RunCommandAsync(targetDir, "build --configuration Release").ConfigureAwait(false);
    }

    public async Task<ProcessResult> TestAsync(string targetDir)
    {
        LogRunningTests(this.logger, targetDir, null);
        return await this.RunCommandAsync(targetDir, "test --configuration Release --no-build").ConfigureAwait(false);
    }

    public async Task<ProcessResult> RunAsync(string targetDir, bool waitForExit = true)
    {
        LogStartingApplication(this.logger, targetDir, null);
        return await this.RunCommandAsync(targetDir, "run --configuration Release", waitForExit).ConfigureAwait(false);
    }

    public async Task<ProcessResult> RunProjectAsync(string targetDir, string projectName, bool waitForExit = true)
    {
        LogStartingProject(this.logger, projectName, targetDir, null);
        return await this.RunCommandAsync(targetDir, $"run --project {projectName} --configuration Release", waitForExit).ConfigureAwait(false);
    }
}