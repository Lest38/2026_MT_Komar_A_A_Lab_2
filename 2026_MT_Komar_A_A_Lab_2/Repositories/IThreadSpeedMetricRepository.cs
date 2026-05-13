namespace Repositories;

using Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IThreadSpeedMetricRepository : IRepository<ThreadSpeedMetric>
{
    Task<IEnumerable<ThreadSpeedMetric>> GetByPerformanceTestIdAsync(int performanceTestId);

    Task<IEnumerable<ThreadSpeedMetric>> GetByHostIdAsync(int hostId);
}