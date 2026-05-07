using Entities;
using Factories;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnitsOfWork;

namespace _2026_MT_Komar_A_A_Lab__.Services;

public class DatabaseRecorder
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DatabaseRecorder> _logger;
    private Project? _currentProject;
    private PipelineStepExecution? _currentStep;

    public DatabaseRecorder(IUnitOfWork unitOfWork, ILogger<DatabaseRecorder> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Project> GetOrCreateProjectAsync(string projectName, string folderPath)
    {
        var existingProject = await _unitOfWork.Projects.GetByFolderPathAsync(folderPath);
        if (existingProject != null)
        {
            _logger.LogInformation("Found existing project: {ProjectName} (Id: {ProjectId})",
                existingProject.Name, existingProject.Id);
            _currentProject = existingProject;
            return existingProject;
        }

        var project = new Project
        {
            Name = projectName,
            FolderPath = folderPath,
            CreatedAt = DateTime.Now,
            Description = $"CI/CD Project - {projectName}"
        };

        await _unitOfWork.Projects.AddAsync(project);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Created new project: {ProjectName} (Id: {ProjectId})",
            project.Name, project.Id);

        _currentProject = project;
        return project;
    }

    public async Task<int> StartPipelineStepAsync(string stageName, string command, string args)
    {
        if (_currentProject == null)
        {
            throw new InvalidOperationException("Project not initialized. Call GetOrCreateProjectAsync first.");
        }

        var stageType = await GetOrCreateStageTypeAsync(stageName);

        _currentStep = new PipelineStepExecution
        {
            ProjectId = _currentProject.Id,
            StageTypeId = stageType.Id,
            Status = "Running",
            StartedAt = DateTime.Now,
            DurationMs = 0,
            ExitCode = 0,
            TotalErrors = 0,
            TotalWarnings = 0
        };

        await _unitOfWork.PipelineStepExecutions.AddAsync(_currentStep);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogDebug("Started recording step: {StageName} (Id: {StepId})", stageName, _currentStep.Id);

        return _currentStep.Id;
    }

    public async Task CompletePipelineStepAsync(int stepId, bool success, int exitCode,
        long durationMs, int errorCount, int warningCount)
    {
        var step = await _unitOfWork.PipelineStepExecutions.GetByIdAsync(stepId);
        if (step == null) return;

        step.Status = success ? "Success" : "Failed";
        step.ExitCode = exitCode;
        step.DurationMs = durationMs;
        step.TotalErrors = errorCount;
        step.TotalWarnings = warningCount;

        await _unitOfWork.PipelineStepExecutions.UpdateAsync(step);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogDebug("Completed step {StepId}: {Status} in {DurationMs}ms",
            stepId, step.Status, durationMs);
    }

    public async Task AddIssueLogAsync(int stepId, string severity, string code, string message)
    {
        var issueLog = new IssueLog
        {
            PipelineStepExecutionId = stepId,
            LoggedAt = DateTime.Now,
            Severity = severity,
            Code = code,
            Message = message
        };

        await _unitOfWork.IssueLogs.AddAsync(issueLog);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AddPerformanceMetricAsync(int stepId, int performanceTestId,
        long sequentialTimeMs, long parallelTimeMs, decimal efficiency)
    {
        var host = await GetOrCreateHostAsync();

        var metric = new ThreadSpeedMetric
        {
            PerformanceTestId = performanceTestId,
            HostId = host.Id,
            PipelineStepExecutionId = stepId,
            SequentialTimeMs = sequentialTimeMs,
            ParallelTimeMs = parallelTimeMs,
            EfficiencyCoefficient = efficiency,
            StartedAt = DateTime.Now,
            DurationMs = parallelTimeMs
        };

        await _unitOfWork.ThreadSpeedMetrics.AddAsync(metric);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<StageType> GetOrCreateStageTypeAsync(string name)
    {
        var existing = await _unitOfWork.StageTypes
            .FindAsync(st => st.Name == name);

        var stageType = existing.FirstOrDefault();

        if (stageType != null) return stageType;

        stageType = new StageType { Name = name };
        await _unitOfWork.StageTypes.AddAsync(stageType);
        await _unitOfWork.SaveChangesAsync();

        return stageType;
    }

    private async Task<Host> GetOrCreateHostAsync()
    {
        var existing = await _unitOfWork.Hosts.GetDefaultHostAsync();
        if (existing != null) return existing;

        var cpuModels = await _unitOfWork.CpuModels.GetAllAsync();
        var cpuModel = cpuModels.FirstOrDefault();

        if (cpuModel == null)
        {
            cpuModel = new CpuModel
            {
                ModelName = "Default CPU",
                PhysicalCoreCount = Environment.ProcessorCount / 2,
                LogicalThreadCount = Environment.ProcessorCount
            };
            await _unitOfWork.CpuModels.AddAsync(cpuModel);
            await _unitOfWork.SaveChangesAsync();
        }

        var host = new Host
        {
            CpuModelId = cpuModel.Id,
            RamGb = 16.00m,
            OperatingSystem = Environment.OSVersion.ToString()
        };

        await _unitOfWork.Hosts.AddAsync(host);
        await _unitOfWork.SaveChangesAsync();

        return host;
    }
}