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
    private static readonly CompositeFormat HostAlreadySeededFmt =
        CompositeFormat.Parse(Utilities.ResourceStrings.Host.AlreadySeeded);

    private static readonly CompositeFormat HostCreatedFmt =
        CompositeFormat.Parse(Utilities.ResourceStrings.Host.Created);

    private static readonly CompositeFormat PerformanceTestAlreadyExistsFmt =
        CompositeFormat.Parse(Utilities.ResourceStrings.PerformanceTest.AlreadyExists);

    private static readonly CompositeFormat PerformanceTestCreatedFmt =
        CompositeFormat.Parse(Utilities.ResourceStrings.PerformanceTest.Created);

    private static readonly CompositeFormat ProjectAlreadyExistsFmt =
        CompositeFormat.Parse(Utilities.ResourceStrings.Project.AlreadyExists);

    private static readonly CompositeFormat ProjectCreatedFmt =
        CompositeFormat.Parse(Utilities.ResourceStrings.Project.Created);

    private static readonly CompositeFormat PipelineStepInfoFmt =
        CompositeFormat.Parse(PipelineStep.StepInfo);

    private static readonly CompositeFormat PipelineIssueLogsCommittedFmt =
        CompositeFormat.Parse(PipelineStep.IssueLogsCommitted);

    private static readonly CompositeFormat PipelineReadBackInfoFmt =
        CompositeFormat.Parse(PipelineStep.ReadBackInfo);

    private static readonly CompositeFormat PipelineLogEntryFmt =
        CompositeFormat.Parse(PipelineStep.LogEntry);

    private static readonly CompositeFormat PipelineTransactionRolledBackFmt =
        CompositeFormat.Parse(PipelineStep.TransactionRolledBack);

    private static readonly CompositeFormat ThreadSpeedMetricAlreadyRecordedFmt =
        CompositeFormat.Parse(Utilities.ResourceStrings.ThreadSpeedMetric.AlreadyRecorded);

    private static readonly CompositeFormat ThreadSpeedMetricSavedFmt =
        CompositeFormat.Parse(Utilities.ResourceStrings.ThreadSpeedMetric.MetricSaved);

    private static readonly CompositeFormat DbSummaryProjectEntryFmt =
        CompositeFormat.Parse(DatabaseSummary.ProjectEntry);

    private static readonly CompositeFormat DbSummaryStepEntryFmt =
        CompositeFormat.Parse(DatabaseSummary.StepEntry);

    private static readonly CompositeFormat DbSummaryPipelineStepsFmt =
        CompositeFormat.Parse(DatabaseSummary.PipelineSteps);

    private static readonly CompositeFormat DbSummaryIssueLogsFmt =
        CompositeFormat.Parse(DatabaseSummary.IssueLogs);

    private static readonly CompositeFormat DbSummaryIssueEntryFmt =
        CompositeFormat.Parse(DatabaseSummary.IssueEntry);

    private static readonly CompositeFormat DbSummaryThreadSpeedMetricsFmt =
        CompositeFormat.Parse(DatabaseSummary.ThreadSpeedMetrics);

    private static readonly CompositeFormat DbSummaryMetricEntryFmt =
        CompositeFormat.Parse(DatabaseSummary.MetricEntry);

    private static readonly CompositeFormat DbSummaryStageTypesFmt =
        CompositeFormat.Parse(DatabaseSummary.StageTypes);

    private static readonly CompositeFormat DbSummaryStageTypeEntryFmt =
        CompositeFormat.Parse(DatabaseSummary.StageTypeEntry);

    private static readonly CompositeFormat DbSummaryCpuModelsFmt =
        CompositeFormat.Parse(DatabaseSummary.CpuModels);

    private static readonly CompositeFormat DbSummaryCpuModelEntryFmt =
        CompositeFormat.Parse(DatabaseSummary.CpuModelEntry);

    private static readonly CompositeFormat DbSummaryHostsFmt =
        CompositeFormat.Parse(DatabaseSummary.Hosts);

    private static readonly CompositeFormat DbSummaryHostEntryFmt =
        CompositeFormat.Parse(DatabaseSummary.HostEntry);

    private static readonly CompositeFormat DbSummaryPerformanceTestsFmt =
        CompositeFormat.Parse(DatabaseSummary.PerformanceTests);

    private static readonly CompositeFormat DbSummaryPerformanceTestEntryFmt =
        CompositeFormat.Parse(DatabaseSummary.PerformanceTestEntry);

    public static async Task Main()
    {
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(Database.ConnectionString));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDataFactory, DefaultDataFactory>();

        var serviceProvider = services.BuildServiceProvider();
        try
        {
            var scope = serviceProvider.CreateAsyncScope();
            try
            {
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
            finally
            {
                await scope.DisposeAsync().ConfigureAwait(false);
            }
        }
        finally
        {
            await serviceProvider.DisposeAsync().ConfigureAwait(false);
        }
    }

    private static async Task<Entities.Host> SeedHostAsync(IUnitOfWork uow, IDataFactory factory)
    {
        var existing = await uow.Hosts.GetDefaultHostAsync().ConfigureAwait(false);
        if (existing is not null)
        {
            Console.WriteLine(string.Format(
                CultureInfo.InvariantCulture,
                HostAlreadySeededFmt,
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
            HostCreatedFmt,
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
            var existing = await uow.PerformanceTests
                .GetByDescriptionAsync(pt.Description).ConfigureAwait(false);

            if (existing is not null)
            {
                Console.WriteLine(string.Format(
                    CultureInfo.InvariantCulture,
                    PerformanceTestAlreadyExistsFmt,
                    pt.Description));
                continue;
            }

            await uow.PerformanceTests.AddAsync(pt).ConfigureAwait(false);
            Console.WriteLine(string.Format(
                CultureInfo.InvariantCulture,
                PerformanceTestCreatedFmt,
                pt.Description));
        }

        await uow.SaveChangesAsync().ConfigureAwait(false);
    }

    private static async Task<Entities.Project> SeedProjectAsync(IUnitOfWork uow)
    {
        var existing = await uow.Projects
            .GetByFolderPathAsync(Utilities.ResourceStrings.Project.DefaultFolderPath).ConfigureAwait(false);

        if (existing is not null)
        {
            Console.WriteLine(string.Format(
                CultureInfo.InvariantCulture,
                ProjectAlreadyExistsFmt,
                existing.Id,
                existing.Name));
            return existing;
        }

        var project = new Entities.Project
        {
            Name = Utilities.ResourceStrings.Project.DefaultName,
            FolderPath = Utilities.ResourceStrings.Project.DefaultFolderPath,
        };
        await uow.Projects.AddAsync(project).ConfigureAwait(false);
        await uow.SaveChangesAsync().ConfigureAwait(false);

        Console.WriteLine(string.Format(
            CultureInfo.InvariantCulture,
            ProjectCreatedFmt,
            project.Id,
            project.Name));
        return project;
    }

    private static async Task DemonstratePipelineWorkflowAsync(
        IUnitOfWork uow, Entities.Project project)
    {
        Banner("Pipeline Workflow Demo");

        var buildStage = await uow.StageTypes.GetByIdAsync(1).ConfigureAwait(false);
        if (buildStage is null)
        {
            Banner(PipelineStep.StageNotFound);
            return;
        }

        var existingSteps = await uow.PipelineStepExecutions
            .GetByProjectIdAsync(project.Id).ConfigureAwait(false);

        if (existingSteps.Any(s => s.StageTypeId == buildStage.Id))
        {
            Banner(PipelineStep.BuildStepAlreadyExists);
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
                PipelineStepInfoFmt,
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

            Console.WriteLine(string.Format(
                CultureInfo.InvariantCulture,
                PipelineIssueLogsCommittedFmt,
                logs.Length));

            var stepWithLogs = await uow.PipelineStepExecutions
                .GetWithLogsAsync(step.Id).ConfigureAwait(false);

            if (stepWithLogs is not null)
            {
                Console.WriteLine(string.Format(
                    CultureInfo.InvariantCulture,
                    PipelineReadBackInfoFmt,
                    stepWithLogs.Id,
                    stepWithLogs.IssueLogs.Count));

                foreach (var log in stepWithLogs.IssueLogs)
                {
                    Console.WriteLine(string.Format(
                        CultureInfo.InvariantCulture,
                        PipelineLogEntryFmt,
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
                PipelineTransactionRolledBackFmt,
                ex.Message));
            throw;
        }
    }

    private static async Task DemonstrateThreadSpeedMetricsAsync(
        IUnitOfWork uow, Entities.Host host)
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

        var hostMetrics = await uow.ThreadSpeedMetrics
            .GetByHostIdAsync(host.Id).ConfigureAwait(false);

        if (hostMetrics.Any(m => m.PerformanceTestId == perfTest.Id))
        {
            Console.WriteLine(string.Format(
                CultureInfo.InvariantCulture,
                ThreadSpeedMetricAlreadyRecordedFmt,
                perfTest.Description));
            return;
        }

        const long seqMs = 8_240;
        const long parMs = 1_340;
        var efficiency = Math.Round((decimal)seqMs / parMs, 4);

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
            ThreadSpeedMetricSavedFmt,
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
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, DbSummaryProjectEntryFmt, p.Id, p.Name));
        }

        var steps = await uow.PipelineStepExecutions.GetAllAsync().ConfigureAwait(false);
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, DbSummaryPipelineStepsFmt, steps.Count()));
        foreach (var s in steps)
        {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, DbSummaryStepEntryFmt, s.Id, s.Status, s.TotalErrors, s.TotalWarnings, s.DurationMs));
        }

        var issueLogs = await uow.IssueLogs.GetAllAsync().ConfigureAwait(false);
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, DbSummaryIssueLogsFmt, issueLogs.Count()));
        foreach (var il in issueLogs)
        {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, DbSummaryIssueEntryFmt, il.Severity, il.Code, il.Message));
        }

        var metrics = await uow.ThreadSpeedMetrics.GetAllAsync().ConfigureAwait(false);
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, DbSummaryThreadSpeedMetricsFmt, metrics.Count()));
        foreach (var m in metrics)
        {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, DbSummaryMetricEntryFmt, m.Id, m.SequentialTimeMs, m.ParallelTimeMs, m.EfficiencyCoefficient));
        }

        var stageTypes = await uow.StageTypes.GetAllAsync().ConfigureAwait(false);
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, DbSummaryStageTypesFmt, stageTypes.Count()));
        foreach (var st in stageTypes)
        {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, DbSummaryStageTypeEntryFmt, st.Id, st.Name));
        }

        var cpuModels = await uow.CpuModels.GetAllAsync().ConfigureAwait(false);
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, DbSummaryCpuModelsFmt, cpuModels.Count()));
        foreach (var c in cpuModels)
        {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, DbSummaryCpuModelEntryFmt, c.Id, c.ModelName, c.PhysicalCoreCount, c.LogicalThreadCount));
        }

        var hosts = await uow.Hosts.GetAllAsync().ConfigureAwait(false);
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, DbSummaryHostsFmt, hosts.Count()));
        foreach (var h in hosts)
        {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, DbSummaryHostEntryFmt, h.Id, h.CpuModelId, h.RamGb, h.OperatingSystem));
        }

        var perfTests = await uow.PerformanceTests.GetAllAsync().ConfigureAwait(false);
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, DbSummaryPerformanceTestsFmt, perfTests.Count()));
        foreach (var pt in perfTests)
        {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, DbSummaryPerformanceTestEntryFmt, pt.Id, pt.Description));
        }
    }

    private static void Banner(string text)
    {
        Console.WriteLine();
        Console.WriteLine(text);
    }
}