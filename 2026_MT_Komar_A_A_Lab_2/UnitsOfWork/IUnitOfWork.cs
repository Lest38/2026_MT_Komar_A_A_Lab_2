using System;
using System.Threading.Tasks;
using Entities;
using Repositories;

namespace UnitsOfWork;

public interface IUnitOfWork : IDisposable
{
    IProjectRepository Projects { get; }

    IRepository<StageType> StageTypes { get; }

    IPipelineStepExecutionRepository PipelineStepExecutions { get; }

    IIssueLogRepository IssueLogs { get; }

    IRepository<CpuModel> CpuModels { get; }

    IHostRepository Hosts { get; }

    IPerformanceTestRepository PerformanceTests { get; }

    IThreadSpeedMetricRepository ThreadSpeedMetrics { get; }

    Task<int> SaveChangesAsync();

    Task BeginTransactionAsync();

    Task CommitTransactionAsync();

    Task RollbackTransactionAsync();
}