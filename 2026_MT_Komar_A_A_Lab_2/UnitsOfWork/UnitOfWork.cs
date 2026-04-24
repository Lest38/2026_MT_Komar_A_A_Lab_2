namespace UnitsOfWork;

using Data;
using Entities;
using Microsoft.EntityFrameworkCore.Storage;
using Repositories;
using System;
using System.Threading.Tasks;

#nullable enable
public class UnitOfWork(ApplicationDbContext context)
    : IUnitOfWork
{
    private readonly ApplicationDbContext context = context;
    private IDbContextTransaction? transaction;
    private bool disposed;

    private IProjectRepository? projects;
    private IRepository<StageType>? stageTypes;
    private IPipelineStepExecutionRepository? pipelineStepExecutions;
    private IIssueLogRepository? issueLogs;
    private IRepository<CpuModel>? cpuModels;
    private IHostRepository? hosts;
    private IPerformanceTestRepository? performanceTests;
    private IThreadSpeedMetricRepository? threadSpeedMetrics;

    public IProjectRepository Projects =>
            this.projects ??= new ProjectRepository(this.context);

    public IRepository<StageType> StageTypes =>
        this.stageTypes ??= new Repository<StageType>(this.context);

    public IPipelineStepExecutionRepository PipelineStepExecutions =>
        this.pipelineStepExecutions ??= new PipelineStepExecutionRepository(this.context);

    public IIssueLogRepository IssueLogs =>
        this.issueLogs ??= new IssueLogRepository(this.context);

    public IRepository<CpuModel> CpuModels =>
        this.cpuModels ??= new Repository<CpuModel>(this.context);

    public IHostRepository Hosts =>
        this.hosts ??= new HostRepository(this.context);

    public IPerformanceTestRepository PerformanceTests =>
        this.performanceTests ??= new PerformanceTestRepository(this.context);

    public IThreadSpeedMetricRepository ThreadSpeedMetrics =>
        this.threadSpeedMetrics ??= new ThreadSpeedMetricRepository(this.context);

    public async Task<int> SaveChangesAsync()
    {
        return await this.context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task BeginTransactionAsync()
    {
        this.transaction = await this.context.Database.BeginTransactionAsync().ConfigureAwait(false);
    }

    public async Task CommitTransactionAsync()
    {
        if (this.transaction != null)
        {
            await this.transaction.CommitAsync().ConfigureAwait(false);
            await this.transaction.DisposeAsync().ConfigureAwait(false);
            this.transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (this.transaction != null)
        {
            await this.transaction.RollbackAsync().ConfigureAwait(false);
            await this.transaction.DisposeAsync().ConfigureAwait(false);
            this.transaction = null;
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed && disposing)
        {
            this.transaction?.Dispose();
            this.context.Dispose();
        }

        this.disposed = true;
    }
}
#nullable restore