using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Entities;
using Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UnitsOfWork;
using Utilities;

namespace DesignTimeDbContextFactory;

public static class Program
{
    private static readonly CompositeFormat HostAlreadySeededFormat = CompositeFormat.Parse(ResourceStrings.Host.AlreadySeeded);
    private static readonly CompositeFormat HostCreatedFormat = CompositeFormat.Parse(ResourceStrings.Host.Created);
    private static readonly CompositeFormat PerformanceTestAlreadyExistsFormat = CompositeFormat.Parse(ResourceStrings.PerformanceTest.AlreadyExists);
    private static readonly CompositeFormat PerformanceTestCreatedFormat = CompositeFormat.Parse(ResourceStrings.PerformanceTest.Created);
    private static readonly CompositeFormat ProjectAlreadyExistsFormat = CompositeFormat.Parse(ResourceStrings.Project.AlreadyExists);
    private static readonly CompositeFormat ProjectCreatedFormat = CompositeFormat.Parse(ResourceStrings.Project.Created);
    private static readonly CompositeFormat PipelineStepInfoFormat = CompositeFormat.Parse(ResourceStrings.PipelineStep.StepInfo);
    private static readonly CompositeFormat PipelineIssueLogsCommittedFormat = CompositeFormat.Parse(ResourceStrings.PipelineStep.IssueLogsCommitted);
    private static readonly CompositeFormat PipelineReadBackInfoFormat = CompositeFormat.Parse(ResourceStrings.PipelineStep.ReadBackInfo);
    private static readonly CompositeFormat PipelineLogEntryFormat = CompositeFormat.Parse(ResourceStrings.PipelineStep.LogEntry);
    private static readonly CompositeFormat PipelineTransactionRolledBackFormat = CompositeFormat.Parse(ResourceStrings.PipelineStep.TransactionRolledBack);
    private static readonly CompositeFormat ThreadSpeedMetricAlreadyRecordedFormat = CompositeFormat.Parse(ResourceStrings.ThreadSpeedMetric.AlreadyRecorded);
    private static readonly CompositeFormat ThreadSpeedMetricSavedFormat = CompositeFormat.Parse(ResourceStrings.ThreadSpeedMetric.MetricSaved);
    private static readonly CompositeFormat DatabaseSummaryProjectEntryFormat = CompositeFormat.Parse(ResourceStrings.DatabaseSummary.ProjectEntry);
    private static readonly CompositeFormat DatabaseSummaryStepEntryFormat = CompositeFormat.Parse(ResourceStrings.DatabaseSummary.StepEntry);
    private static readonly CompositeFormat DatabaseSummaryPipelineStepsFormat = CompositeFormat.Parse(ResourceStrings.DatabaseSummary.PipelineSteps);
    private static readonly CompositeFormat DatabaseSummaryIssueLogsFormat = CompositeFormat.Parse(ResourceStrings.DatabaseSummary.IssueLogs);
    private static readonly CompositeFormat DatabaseSummaryIssueEntryFormat = CompositeFormat.Parse(ResourceStrings.DatabaseSummary.IssueEntry);
    private static readonly CompositeFormat DatabaseSummaryThreadSpeedMetricsFormat = CompositeFormat.Parse(ResourceStrings.DatabaseSummary.ThreadSpeedMetrics);
    private static readonly CompositeFormat DatabaseSummaryMetricEntryFormat = CompositeFormat.Parse(ResourceStrings.DatabaseSummary.MetricEntry);
    private static readonly CompositeFormat DatabaseSummaryStageTypesFormat = CompositeFormat.Parse(ResourceStrings.DatabaseSummary.StageTypes);
    private static readonly CompositeFormat DatabaseSummaryStageTypeEntryFormat = CompositeFormat.Parse(ResourceStrings.DatabaseSummary.StageTypeEntry);
    private static readonly CompositeFormat DatabaseSummaryCpuModelsFormat = CompositeFormat.Parse(ResourceStrings.DatabaseSummary.CpuModels);
    private static readonly CompositeFormat DatabaseSummaryCpuModelEntryFormat = CompositeFormat.Parse(ResourceStrings.DatabaseSummary.CpuModelEntry);
    private static readonly CompositeFormat DatabaseSummaryHostsFormat = CompositeFormat.Parse(ResourceStrings.DatabaseSummary.Hosts);
    private static readonly CompositeFormat DatabaseSummaryHostEntryFormat = CompositeFormat.Parse(ResourceStrings.DatabaseSummary.HostEntry);
    private static readonly CompositeFormat DatabaseSummaryPerformanceTestsFormat = CompositeFormat.Parse(ResourceStrings.DatabaseSummary.PerformanceTests);
    private static readonly CompositeFormat DatabaseSummaryPerformanceTestEntryFormat = CompositeFormat.Parse(ResourceStrings.DatabaseSummary.PerformanceTestEntry);
    private static readonly CompositeFormat BannerFormat = CompositeFormat.Parse(ResourceStrings.ConsoleMessages.BannerFormat);

    public static async Task Main()
    {
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(ResourceStrings.Database.ConnectionString));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDataFactory, DefaultDataFactory>();

        await using var serviceProvider = services.BuildServiceProvider();
        await using var scope = serviceProvider.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var factory = scope.ServiceProvider.GetRequiredService<IDataFactory>();

        await dbContext.Database.MigrateAsync().ConfigureAwait(false);

        Banner(ResourceStrings.Database.MigratedUpToDate);

        var host = await SeedHostAsync(unitOfWork, factory).ConfigureAwait(false);
        await SeedPerformanceTestsAsync(unitOfWork, factory).ConfigureAwait(false);
        var project = await SeedProjectAsync(unitOfWork).ConfigureAwait(false);
        await DemonstratePipelineWorkflowAsync(unitOfWork, project).ConfigureAwait(false);
        await DemonstrateThreadSpeedMetricsAsync(unitOfWork, host).ConfigureAwait(false);
        await PrintDatabaseSummaryAsync(unitOfWork).ConfigureAwait(false);
    }

    private static async Task<Host> SeedHostAsync(IUnitOfWork uow, IDataFactory factory)
    {
        var existing = await uow.Hosts.GetDefaultHostAsync().ConfigureAwait(false);
        if (existing is not null)
        {
            Console.WriteLine(string.Format(
                CultureInfo.InvariantCulture,
                HostAlreadySeededFormat.ToString(),
                existing.Id,
                existing.OperatingSystem,
                existing.RamGb));
            return existing;
        }

        var host = factory.CreateHost();
        await uow.Hosts.AddAsync(host).ConfigureAwait(false);
        await uow.SaveChangesAsync().ConfigureAwait(false);
        Console.WriteLine(string.Format(
            CultureInfo.InvariantCulture,
            HostCreatedFormat.ToString(),
            host.Id,
            host.CpuModelId,
            host.RamGb,
            host.OperatingSystem));
        return host;
    }

    private static async Task SeedPerformanceTestsAsync(IUnitOfWork uow, IDataFactory factory)
    {
        foreach (var pt in factory.CreatePerformanceTests())
        {
            var existing = await uow.PerformanceTests.GetByDescriptionAsync(pt.Description).ConfigureAwait(false);
            if (existing is not null)
            {
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, PerformanceTestAlreadyExistsFormat.ToString(), pt.Description));
                continue;
            }

            await uow.PerformanceTests.AddAsync(pt).ConfigureAwait(false);
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, PerformanceTestCreatedFormat.ToString(), pt.Description));
        }

        await uow.SaveChangesAsync().ConfigureAwait(false);
    }

    private static async Task<Project> SeedProjectAsync(IUnitOfWork uow)
    {
        var existing = await uow.Projects.GetByFolderPathAsync(ResourceStrings.Project.DefaultFolderPath).ConfigureAwait(false);
        if (existing is not null)
        {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, ProjectAlreadyExistsFormat.ToString(), existing.Id, existing.Name));
            return existing;
        }

        var project = new Project
        {
            Name = ResourceStrings.Project.DefaultName,
            FolderPath = ResourceStrings.Project.DefaultFolderPath,
        };
        await uow.Projects.AddAsync(project).ConfigureAwait(false);
        await uow.SaveChangesAsync().ConfigureAwait(false);
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, ProjectCreatedFormat.ToString(), project.Id, project.Name));
        return project;
    }

    private static async Task DemonstratePipelineWorkflowAsync(IUnitOfWork uow, Project project)
    {
        Banner("Pipeline Workflow Demo");

        var buildStage = await uow.StageTypes.GetByIdAsync(1).ConfigureAwait(false);
        if (buildStage is null)
        {
            Banner(ResourceStrings.PipelineStep.StageNotFound);
            return;
        }

        await uow.BeginTransactionAsync().ConfigureAwait(false);
        try
        {
            var step = new PipelineStepExecution
            {
                ProjectId = project.Id,
                StageTypeId = buildStage.Id,
                Status = "Failed",
                StartedAt = DateTime.UtcNow,
                DurationMs = 3_742,
                ExitCode = 1,
                TotalErrors = 2,
                TotalWarnings = 3,
            };
            await uow.PipelineStepExecutions.AddAsync(step).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);

            Console.WriteLine(string.Format(
                CultureInfo.InvariantCulture,
                PipelineStepInfoFormat.ToString(),
                step.Id,
                buildStage.Name,
                step.Status,
                step.DurationMs));

            var logs = new[]
            {
                new IssueLog
                {
                    PipelineStepExecutionId = step.Id,
                    LoggedAt = DateTime.UtcNow,
                    Severity = "Error",
                    Code = "CS0246",
                    Message = "The type or namespace name 'Foo' could not be found.",
                },
                new IssueLog
                {
                    PipelineStepExecutionId = step.Id,
                    LoggedAt = DateTime.UtcNow.AddMilliseconds(10),
                    Severity = "Error",
                    Code = "CS0103",
                    Message = "The name 'bar' does not exist in the current context.",
                },
            };

            foreach (var log in logs)
            {
                await uow.IssueLogs.AddAsync(log).ConfigureAwait(false);
            }

            await uow.SaveChangesAsync().ConfigureAwait(false);
            await uow.CommitTransactionAsync().ConfigureAwait(false);

            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, PipelineIssueLogsCommittedFormat.ToString(), logs.Length));

            var stepWithLogs = await uow.PipelineStepExecutions
                .GetWithLogsAsync(step.Id).ConfigureAwait(false);

            if (stepWithLogs is not null)
            {
                Console.WriteLine(string.Format(
                    CultureInfo.InvariantCulture,
                    PipelineReadBackInfoFormat.ToString(),
                    stepWithLogs.Id,
                    stepWithLogs.IssueLogs.Count));
                foreach (var log in stepWithLogs.IssueLogs)
                {
                    Console.WriteLine(string.Format(
                        CultureInfo.InvariantCulture,
                        PipelineLogEntryFormat.ToString(),
                        log.Severity,
                        log.Code,
                        log.Message));
                }
            }
        }
        catch (Exception ex)
        {
            await uow.RollbackTransactionAsync().ConfigureAwait(false);
            Console.WriteLine(string.Format(
                CultureInfo.InvariantCulture,
                PipelineTransactionRolledBackFormat.ToString(),
                ex.Message));
            throw;
        }
    }

    private static async Task DemonstrateThreadSpeedMetricsAsync(IUnitOfWork uow, Host host)
    {
        Banner(ResourceStrings.ThreadSpeedMetric.DemoTitle);

        var allTests = await uow.PerformanceTests.GetAllAsync().ConfigureAwait(false);
        var perfTest = allTests.FirstOrDefault();
        if (perfTest is null)
        {
            Banner(ResourceStrings.ThreadSpeedMetric.NoPerformanceTestFound);
            return;
        }

        var allSteps = await uow.PipelineStepExecutions.GetAllAsync().ConfigureAwait(false);
        var step = allSteps.FirstOrDefault();
        if (step is null)
        {
            Banner(ResourceStrings.ThreadSpeedMetric.NoPipelineStepFound);
            return;
        }

        var existing = await uow.ThreadSpeedMetrics
            .FindAsync(m => m.HostId == host.Id && m.PerformanceTestId == perfTest.Id)
            .ConfigureAwait(false);

        if (existing.Any())
        {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, ThreadSpeedMetricAlreadyRecordedFormat.ToString(), perfTest.Description));
            return;
        }

        long seqMs = 8_240;
        long parMs = 1_340;
        decimal efficiency = Math.Round((decimal)seqMs / parMs, 4);

        var metric = new ThreadSpeedMetric
        {
            PerformanceTestId = perfTest.Id,
            HostId = host.Id,
            PipelineStepExecutionId = step.Id,
            SequentialTimeMs = seqMs,
            ParallelTimeMs = parMs,
            EfficiencyCoefficient = efficiency,
            StartedAt = DateTime.UtcNow,
            DurationMs = seqMs + parMs,
        };

        await uow.ThreadSpeedMetrics.AddAsync(metric).ConfigureAwait(false);
        await uow.SaveChangesAsync().ConfigureAwait(false);

        Console.WriteLine(string.Format(
            CultureInfo.InvariantCulture,
            ThreadSpeedMetricSavedFormat.ToString(),
            perfTest.Description,
            seqMs,
            parMs,
            efficiency,
            metric.Id));
    }

    private static async Task PrintDatabaseSummaryAsync(IUnitOfWork uow)
    {
        Banner(ResourceStrings.DatabaseSummary.Header);

        var projects = await uow.Projects.GetAllAsync().ConfigureAwait(false);
        Banner(ResourceStrings.DatabaseSummary.Projects);
        foreach (var p in projects)
        {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, DatabaseSummaryProjectEntryFormat.ToString(), p.Id, p.Name));
        }

        var steps = await uow.PipelineStepExecutions.GetAllAsync().ConfigureAwait(false);
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, DatabaseSummaryPipelineStepsFormat.ToString(), steps.Count()));
        foreach (var s in steps)
        {
            Console.WriteLine(string.Format(
                CultureInfo.InvariantCulture,
                DatabaseSummaryStepEntryFormat.ToString(),
                s.Id,
                s.Status,
                s.TotalErrors,
                s.TotalWarnings,
                s.DurationMs));
        }

        var issueLogs = await uow.IssueLogs.GetAllAsync().ConfigureAwait(false);
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, DatabaseSummaryIssueLogsFormat.ToString(), issueLogs.Count()));
        foreach (var il in issueLogs)
        {
            Console.WriteLine(string.Format(
                CultureInfo.InvariantCulture,
                DatabaseSummaryIssueEntryFormat.ToString(),
                il.Severity,
                il.Code,
                il.Message));
        }

        var metrics = await uow.ThreadSpeedMetrics.GetAllAsync().ConfigureAwait(false);
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, DatabaseSummaryThreadSpeedMetricsFormat.ToString(), metrics.Count()));
        foreach (var m in metrics)
        {
            Console.WriteLine(string.Format(
                CultureInfo.InvariantCulture,
                DatabaseSummaryMetricEntryFormat.ToString(),
                m.Id,
                m.SequentialTimeMs,
                m.ParallelTimeMs,
                m.EfficiencyCoefficient));
        }

        var stageTypes = await uow.StageTypes.GetAllAsync().ConfigureAwait(false);
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, DatabaseSummaryStageTypesFormat.ToString(), stageTypes.Count()));
        foreach (var st in stageTypes)
        {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, DatabaseSummaryStageTypeEntryFormat.ToString(), st.Id, st.Name));
        }

        var cpuModels = await uow.CpuModels.GetAllAsync().ConfigureAwait(false);
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, DatabaseSummaryCpuModelsFormat.ToString(), cpuModels.Count()));
        foreach (var c in cpuModels)
        {
            Console.WriteLine(string.Format(
                CultureInfo.InvariantCulture,
                DatabaseSummaryCpuModelEntryFormat.ToString(),
                c.Id,
                c.ModelName,
                c.PhysicalCoreCount,
                c.LogicalThreadCount));
        }

        var hosts = await uow.Hosts.GetAllAsync().ConfigureAwait(false);
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, DatabaseSummaryHostsFormat.ToString(), hosts.Count()));
        foreach (var h in hosts)
        {
            Console.WriteLine(string.Format(
                CultureInfo.InvariantCulture,
                DatabaseSummaryHostEntryFormat.ToString(),
                h.Id,
                h.CpuModelId,
                h.RamGb,
                h.OperatingSystem));
        }

        var perfTests = await uow.PerformanceTests.GetAllAsync().ConfigureAwait(false);
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, DatabaseSummaryPerformanceTestsFormat.ToString(), perfTests.Count()));
        foreach (var pt in perfTests)
        {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, DatabaseSummaryPerformanceTestEntryFormat.ToString(), pt.Id, pt.Description));
        }
    }

    private static void Banner(string text)
    {
        Console.WriteLine();
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, BannerFormat.ToString(), text));
    }
}