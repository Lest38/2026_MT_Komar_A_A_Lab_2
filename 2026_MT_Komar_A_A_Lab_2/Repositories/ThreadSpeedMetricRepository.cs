namespace Repositories;

using Data;
using Factories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ThreadSpeedMetricRepository(ApplicationDbContext context)
    : Repository<ThreadSpeedMetric>(context), IThreadSpeedMetricRepository
{
    public async Task<IEnumerable<ThreadSpeedMetric>> GetByPerformanceTestIdAsync(int performanceTestId)
    {
        return await this.DbSet
            .Include(tsm => tsm.Host)
            .ThenInclude(h => h!.CpuModel)
            .Include(tsm => tsm.PerformanceTest)
            .Where(tsm => tsm.PerformanceTestId == performanceTestId)
            .OrderByDescending(tsm => tsm.StartedAt)
            .ToListAsync().ConfigureAwait(false);
    }

    public async Task<IEnumerable<ThreadSpeedMetric>> GetByHostIdAsync(int hostId)
    {
        return await this.DbSet
            .Include(tsm => tsm.PerformanceTest)
            .Include(tsm => tsm.Host)
            .ThenInclude(h => h!.CpuModel)
            .Where(tsm => tsm.HostId == hostId)
            .OrderByDescending(tsm => tsm.StartedAt)
            .ToListAsync().ConfigureAwait(false);
    }
}