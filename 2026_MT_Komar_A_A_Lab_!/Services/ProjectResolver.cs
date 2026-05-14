using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Services;

#nullable enable
public class ProjectResolver(ILogger<ProjectResolver> logger)
{
    private static readonly Action<ILogger, string, Exception?> LogAutoSelectedProject =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(5001, nameof(ResolveProjectIfNeeded)),
            "Auto-selected project: {ProjectName}");

    private static readonly Action<ILogger, Exception?> LogMultipleProjectsFound =
        LoggerMessage.Define(
            LogLevel.Warning,
            new EventId(5002, nameof(ResolveProjectIfNeeded)),
            "Multiple projects found, please specify --project in args:");

    private static readonly Action<ILogger, string, Exception?> LogProjectWarning =
        LoggerMessage.Define<string>(
            LogLevel.Warning,
            new EventId(5003, nameof(ResolveProjectIfNeeded)),
            "  - {Project}");

    private readonly ILogger<ProjectResolver> logger = logger;

    public static string[] FindProjectFiles(string targetDir)
    {
        var projectFiles = Directory.GetFiles(targetDir, "*.csproj", SearchOption.TopDirectoryOnly);

        if (projectFiles.Length == 0)
        {
            projectFiles = Directory.GetFiles(targetDir, "*.csproj", SearchOption.AllDirectories);
        }

        return projectFiles;
    }

    public string ResolveProjectIfNeeded(string targetDir, string args)
    {
        ArgumentNullException.ThrowIfNull(args);

        if (args.Contains("--project", StringComparison.Ordinal))
        {
            return args;
        }

        var projectFiles = FindProjectFiles(targetDir);

        if (projectFiles.Length == 1)
        {
            var projectName = Path.GetFileName(projectFiles[0]);
            LogAutoSelectedProject(this.logger, projectName, null);
            return $"{args} --project {projectName}";
        }

        if (projectFiles.Length > 1)
        {
            LogMultipleProjectsFound(this.logger, null);
            foreach (var proj in projectFiles)
            {
                LogProjectWarning(this.logger, Path.GetFileName(proj), null);
            }
        }

        return args;
    }
}