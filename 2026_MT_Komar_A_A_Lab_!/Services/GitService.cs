using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Models;

namespace Services;

#nullable enable
public class GitService(ILogger<GitService> logger)
{
    private static readonly Action<ILogger, string, int, Exception?> LogTargetDirectoryNotEmpty =
        LoggerMessage.Define<string, int>(
            LogLevel.Warning,
            new EventId(4001, nameof(CloneAsync)),
            "Target directory {TargetDir} is not empty. Found {FileCount} files.");

    private static readonly Action<ILogger, string, Exception?> LogCreatedParentDirectory =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(4002, nameof(CloneAsync)),
            "Created parent directory: {ParentDir}");

    private static readonly Action<ILogger, string, string, Exception?> LogCloningRepository =
        LoggerMessage.Define<string, string>(
            LogLevel.Information,
            new EventId(4003, nameof(CloneAsync)),
            "Cloning repository {RepoUrl} to {TargetDir}");

    private static readonly Action<ILogger, string, Exception?> LogPullingChanges =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(4004, nameof(PullAsync)),
            "Pulling latest changes in {TargetDir}");

    private readonly ILogger<GitService> logger = logger;

    public static async Task<ProcessResult> GetCurrentBranchAsync(string targetDir)
    {
        if (!TryEnsureDirectoryExists(targetDir, out var errorResult))
        {
            return errorResult!;
        }

        return await ProcessRunner.RunCommandAsync(
            "git",
            "rev-parse --abbrev-ref HEAD",
            targetDir)
            .ConfigureAwait(false);
    }

    public static async Task<ProcessResult> GetStatusAsync(string targetDir)
    {
        if (!TryEnsureDirectoryExists(targetDir, out var errorResult))
        {
            return errorResult!;
        }

        return await ProcessRunner.RunCommandAsync(
            "git",
            "status --short",
            targetDir)
            .ConfigureAwait(false);
    }

    public async Task<ProcessResult> CloneAsync(string targetDir, Uri repoUrl)
    {
        ArgumentNullException.ThrowIfNull(repoUrl);

        string cloneTargetDir = targetDir;
        string? repoSubDir;

        var parts = repoUrl.OriginalString.Split([' '], StringSplitOptions.RemoveEmptyEntries);
        string repoUrlString = repoUrl.OriginalString;
        if (parts.Length > 1)
        {
            repoUrlString = parts[0];
            repoSubDir = parts[1];
            cloneTargetDir = Path.Combine(targetDir, repoSubDir);
        }

        if (Directory.Exists(cloneTargetDir))
        {
            var files = Directory.GetFileSystemEntries(cloneTargetDir).ToArray();

            if (files.Length > 0)
            {
                LogTargetDirectoryNotEmpty(this.logger, cloneTargetDir, files.Length, null);

                return CreateSuccessResult(
                    "git",
                    $"clone {repoUrlString}",
                    $"Directory {cloneTargetDir} already exists and contains {files.Length} files. Skipping clone.");
            }
        }
        else
        {
            var parentDir = Path.GetDirectoryName(cloneTargetDir);
            if (!string.IsNullOrEmpty(parentDir) && !Directory.Exists(parentDir))
            {
                Directory.CreateDirectory(parentDir);
                LogCreatedParentDirectory(this.logger, parentDir, null);
            }
        }

        LogCloningRepository(this.logger, repoUrlString, cloneTargetDir, null);
        return await ProcessRunner.RunCommandAsync(
            "git",
            $"clone {repoUrlString} {cloneTargetDir}",
            ".",
            timeoutSeconds: TimeoutDefaults.Clone)
            .ConfigureAwait(false);
    }

    public async Task<ProcessResult> CloneAsync(string targetDir, string repoUrl)
    {
        repoUrl ??= string.Empty;
        if (Uri.TryCreate(repoUrl.Split([' '], StringSplitOptions.RemoveEmptyEntries)[0], UriKind.Absolute, out var uri))
        {
            return await this.CloneAsync(targetDir, uri).ConfigureAwait(false);
        }
        else
        {
            return CreateErrorResult("clone", targetDir, $"Invalid repository URL: {repoUrl}");
        }
    }

    public async Task<ProcessResult> PullAsync(string targetDir)
    {
        if (!TryEnsureDirectoryExists(targetDir, out var errorResult))
        {
            return errorResult!;
        }

        LogPullingChanges(this.logger, targetDir, null);
        return await ProcessRunner.RunCommandAsync(
            "git",
            "pull",
            targetDir,
            timeoutSeconds: TimeoutDefaults.Pull)
            .ConfigureAwait(false);
    }

    private static bool TryEnsureDirectoryExists(string targetDir, out ProcessResult? errorResult)
    {
        if (!Directory.Exists(targetDir))
        {
            errorResult = CreateErrorResult("command", targetDir, "Directory does not exist");
            return false;
        }

        errorResult = null;
        return true;
    }

    private static ProcessResult CreateErrorResult(string command, string targetDir, string errorMessage)
    {
        return new ProcessResult
        {
            Command = "git",
            Arguments = command,
            ExitCode = -1,
            Errors = $"{errorMessage}: {targetDir}",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now,
        };
    }

    private static ProcessResult CreateSuccessResult(string command, string arguments, string output)
    {
        return new ProcessResult
        {
            Command = command,
            Arguments = arguments,
            ExitCode = 0,
            Output = output,
            StartTime = DateTime.Now,
            EndTime = DateTime.Now,
        };
    }
}
#nullable restore