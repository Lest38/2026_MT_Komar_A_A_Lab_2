using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Models;

namespace Services;

#nullable enable
public class ConfigReader
{
    private static readonly Action<ILogger, string, Exception?> LogReadingConfiguration =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(2001, nameof(ReadConfig)),
            "Reading configuration from: {FilePath}");

    private static readonly Action<ILogger, string, Exception?> LogConfigurationFileNotFound =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(2002, nameof(ReadConfig)),
            "Configuration file not found: {FilePath}");

    private static readonly Action<ILogger, string, Exception?> LogLoadedJson =
        LoggerMessage.Define<string>(
            LogLevel.Debug,
            new EventId(2003, nameof(ReadConfig)),
            "Loaded JSON: {JsonString}");

    private static readonly Action<ILogger, Exception?> LogDeserializationFailed =
        LoggerMessage.Define(
            LogLevel.Error,
            new EventId(2004, nameof(ReadConfig)),
            "Failed to deserialize configuration");

    private static readonly Action<ILogger, int, Exception?> LogPipelineLoaded =
        LoggerMessage.Define<int>(
            LogLevel.Information,
            new EventId(2005, nameof(ReadConfig)),
            "Successfully loaded pipeline with {Count} stages");

    private static readonly Action<ILogger, Exception> LogInvalidJson =
        LoggerMessage.Define(
            LogLevel.Error,
            new EventId(2006, nameof(ReadConfig)),
            "Invalid JSON format in configuration file");

    private readonly ILogger<ConfigReader> logger;
    private readonly JsonSerializerOptions jsonOptions;

    public ConfigReader(ILogger<ConfigReader> logger)
    {
        this.logger = logger;
        this.jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
    }

    public PipelineConfig? ReadConfig(string filePath)
    {
        try
        {
            LogReadingConfiguration(this.logger, filePath, null);

            if (!File.Exists(filePath))
            {
                LogConfigurationFileNotFound(this.logger, filePath, null);
                return null;
            }

            string jsonString = File.ReadAllText(filePath);
            LogLoadedJson(this.logger, jsonString, null);

            var config = JsonSerializer.Deserialize<PipelineConfig>(jsonString, this.jsonOptions);

            if (config == null)
            {
                LogDeserializationFailed(this.logger, null);
                return null;
            }

            ValidateConfig(config);

            LogPipelineLoaded(this.logger, config.Pipeline.Count, null);
            return config;
        }
        catch (JsonException ex)
        {
            LogInvalidJson(this.logger, ex);
            return null;
        }
    }

    private static void ValidateConfig(PipelineConfig config)
    {
        if (config.Pipeline == null || config.Pipeline.Count == 0)
        {
            throw new InvalidDataException("Pipeline must contain at least one stage");
        }

        foreach (var stage in config.Pipeline)
        {
            if (string.IsNullOrEmpty(stage.Name))
            {
                throw new InvalidDataException("Each pipeline stage must have a name");
            }

            if (string.IsNullOrEmpty(stage.Command))
            {
                throw new InvalidDataException($"Stage '{stage.Name}' must have a command");
            }
        }
    }
}
#nullable disable