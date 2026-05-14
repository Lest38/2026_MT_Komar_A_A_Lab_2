using System;
using Microsoft.Extensions.Logging;
using Models;

namespace Services;

#nullable enable
public class ConfigurationService(ILogger<ConfigurationService> logger, ConfigReader configReader)
{
    private static readonly Action<ILogger, string, Exception?> LogFailedToLoadConfig =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(3001, nameof(LoadConfiguration)),
            "Failed to load configuration from {ConfigPath}");

    private static readonly Action<ILogger, int, Exception?> LogLoadedStages =
        LoggerMessage.Define<int>(
            LogLevel.Information,
            new EventId(3002, nameof(LoadConfiguration)),
            "Loaded {StageCount} stages from configuration");

    private static readonly Action<ILogger, string, string, string, bool, Exception?> LogStageDebug =
        LoggerMessage.Define<string, string, string, bool>(
            LogLevel.Debug,
            new EventId(3003, nameof(LoadConfiguration)),
            "Stage: {StageName} | Command: {Command} {Args} | StopOnFailure: {StopOnFailure}");

    private readonly ILogger<ConfigurationService> logger = logger;
    private readonly ConfigReader configReader = configReader;

    public PipelineConfig LoadConfiguration(string configPath)
    {
        var config = this.configReader.ReadConfig(configPath);

        if (config == null)
        {
            LogFailedToLoadConfig(this.logger, configPath, null);
            throw new InvalidOperationException($"Failed to load configuration from {configPath}");
        }

        LogLoadedStages(this.logger, config.Pipeline.Count, null);

        foreach (var stage in config.Pipeline)
        {
            LogStageDebug(this.logger, stage.Name, stage.Command, stage.Args, stage.StopOnFailure, null);
        }

        return config;
    }
}