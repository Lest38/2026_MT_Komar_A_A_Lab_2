namespace Repositories;

using Data;
using Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class PerformanceTestRepository(ApplicationDbContext context)
    : Repository<PerformanceTest>(context), IPerformanceTestRepository
{
    public async Task<PerformanceTest?> GetByDescriptionAsync(string description)
    {
        return await this.DbSet.FirstOrDefaultAsync(pt => pt.Description == description).ConfigureAwait(false);
    }
}