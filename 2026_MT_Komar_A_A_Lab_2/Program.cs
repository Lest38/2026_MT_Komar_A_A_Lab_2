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
using Utilities.ResourceStrings;

namespace DesignTimeDbContextFactory;

public static class Program
{
    private const string HostAlreadySeededFormat = Utilities.ResourceStrings.Host.AlreadySeeded;
    private const string HostCreatedFormat = Utilities.ResourceStrings.Host.Created;
    private const string PerformanceTestAlreadyExistsFormat = Utilities.ResourceStrings.PerformanceTest.AlreadyExists;
    private const string PerformanceTestCreatedFormat = Utilities.ResourceStrings.PerformanceTest.Created;
    private const string ProjectAlreadyExistsFormat = Utilities.ResourceStrings.Project.AlreadyExists;
    private const string ProjectCreatedFormat = Utilities.ResourceStrings.Project.Created;
    private const string PipelineStepInfoFormat = Utilities.ResourceStrings.PipelineStep.StepInfo;
    private const string PipelineIssueLogsCommittedFormat = Utilities.ResourceStrings.PipelineStep.IssueLogsCommitted;
    private const string PipelineReadBackInfoFormat = Utilities.ResourceStrings.PipelineStep.ReadBackInfo;
    private const string PipelineLogEntryFormat = Utilities.ResourceStrings.PipelineStep.LogEntry;
    private const string PipelineTransactionRolledBackFormat = Utilities.ResourceStrings.PipelineStep.TransactionRolledBack;
    private const string ThreadSpeedMetricAlreadyRecordedFormat = Utilities.ResourceStrings.ThreadSpeedMetric.AlreadyRecorded;
    private const string ThreadSpeedMetricSavedFormat = Utilities.ResourceStrings.ThreadSpeedMetric.MetricSaved;
    private const string DatabaseSummaryProjectEntryFormat = Utilities.ResourceStrings.DatabaseSummary.ProjectEntry;
    private const string DatabaseSummaryStepEntryFormat = Utilities.ResourceStrings.DatabaseSummary.StepEntry;
    private const string DatabaseSummaryPipelineStepsFormat = Utilities.ResourceStrings.DatabaseSummary.PipelineSteps;
    private const string DatabaseSummaryIssueLogsFormat = Utilities.ResourceStrings.DatabaseSummary.IssueLogs;
    private const string DatabaseSummaryIssueEntryFormat = Utilities.ResourceStrings.DatabaseSummary.IssueEntry;
    private const string DatabaseSummaryThreadSpeedMetricsFormat = Utilities.ResourceStrings.DatabaseSummary.ThreadSpeedMetrics;
    private const string DatabaseSummaryMetricEntryFormat = Utilities.ResourceStrings.DatabaseSummary.MetricEntry;
    private const string DatabaseSummaryStageTypesFormat = Utilities.ResourceStrings.DatabaseSummary.StageTypes;
    private const string DatabaseSummaryStageTypeEntryFormat = Utilities.ResourceStrings.DatabaseSummary.StageTypeEntry;
    private const string DatabaseSummaryCpuModelsFormat = Utilities.ResourceStrings.DatabaseSummary.CpuModels;
    private const string DatabaseSummaryCpuModelEntryFormat = Utilities.ResourceStrings.DatabaseSummary.CpuModelEntry;
    private const string DatabaseSummaryHostsFormat = Utilities.ResourceStrings.DatabaseSummary.Hosts;
    private const string DatabaseSummaryHostEntryFormat = Utilities.ResourceStrings.DatabaseSummary.HostEntry;
    private const string DatabaseSummaryPerformanceTestsFormat = Utilities.ResourceStrings.DatabaseSummary.PerformanceTests;
    private const string DatabaseSummaryPerformanceTestEntryFormat = Utilities.ResourceStrings.DatabaseSummary.PerformanceTestEntry;

    public static async Task Main()
    {
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(Database.ConnectionString));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDataFactory, DefaultDataFactory>();

        await using var serviceProvider = services.BuildServiceProvider();
        await using var scope = serviceProvider.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var factory = scope.ServiceProvider.GetRequiredService<IDataFactory>();

        await dbContext.Database.MigrateAsync().ConfigureAwait(false);

        Banner(Database.MigratedUpToDate);

        var host = await SeedHostAsync(unitOfWork, factory).ConfigureAwait(false);
        await SeedPerformanceTestsAsync(unitOfWork, factory).ConfigureAwait(false);
        var project = await SeedProjectAsync(unitOfWork).ConfigureAwait(false);
        await DemonstratePipelineWorkflowAsync(unitOfWork, project).ConfigureAwait(false);
        await DemonstrateThreadSpeedMetricsAsync(unitOfWork, host).ConfigureAwait(false);
        await PrintDatabaseSummaryAsync(unitOfWork).ConfigureAwait(false);
    }

    private static async Task<Entities.Host> SeedHostAsync(IUnitOfWork uow, IDataFactory factory)
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

    private static async Task<Entities.Project> SeedProjectAsync(IUnitOfWork uow)
    {
        var existing = await uow.Projects.GetByFolderPathAsync(Utilities.ResourceStrings.Project.DefaultFolderPath).ConfigureAwait(false);
        if (existing is not null)
        {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, ProjectAlreadyExistsFormat.ToString(), existing.Id, existing.Name));
            return existing;
        }

        var project = new Entities.Project
        {
            Name = Utilities.ResourceStrings.Project.DefaultName,
            FolderPath = Utilities.ResourceStrings.Project.DefaultFolderPath,
        };
        await uow.Projects.AddAsync(project).ConfigureAwait(false);
        await uow.SaveChangesAsync().ConfigureAwait(false);
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, ProjectCreatedFormat.ToString(), project.Id, project.Name));
        return project;
    }

    private static async Task DemonstratePipelineWorkflowAsync(IUnitOfWork uow, Entities.Project project)
    {
        Banner("Pipeline Workflow Demo");

        var buildStage = await uow.StageTypes.GetByIdAsync(1).ConfigureAwait(false);
        if (buildStage is null)
        {
            Banner(Utilities.ResourceStrings.PipelineStep.StageNotFound);
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

    private static async Task DemonstrateThreadSpeedMetricsAsync(IUnitOfWork uow, Entities.Host host)
    {
        Banner(Utilities.ResourceStrings.ThreadSpeedMetric.DemoTitle);

        var allTests = await uow.PerformanceTests.GetAllAsync().ConfigureAwait(false);
        var perfTest = allTests.FirstOrDefault();
        if (perfTest is null)
        {
            Banner(Utilities.ResourceStrings.ThreadSpeedMetric.NoPerformanceTestFound);
            return;
        }

        var allSteps = await uow.PipelineStepExecutions.GetAllAsync().ConfigureAwait(false);
        var step = allSteps.FirstOrDefault();
        if (step is null)
        {
            Banner(Utilities.ResourceStrings.ThreadSpeedMetric.NoPipelineStepFound);
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

        var metric = new Entities.ThreadSpeedMetric
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
        Banner(DatabaseSummary.Header);

        var projects = await uow.Projects.GetAllAsync().ConfigureAwait(false);
        Banner(DatabaseSummary.Projects);
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
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, text));
    }
}