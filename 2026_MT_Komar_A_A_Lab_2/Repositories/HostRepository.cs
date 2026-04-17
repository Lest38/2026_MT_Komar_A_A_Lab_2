namespace Repositories;

using Data;
using Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

#nullable enable
public class HostRepository(ApplicationDbContext context)
    : Repository<Host>(context), IHostRepository
{
    public async Task<Host?> GetDefaultHostAsync()
    {
        return await this.DbSet
            .Include(h => h.CpuModel)
            .FirstOrDefaultAsync().ConfigureAwait(false);
    }

    public async Task<Host?> GetWithCpuModelAsync(int id)
    {
        return await this.DbSet
            .Include(h => h.CpuModel)
            .FirstOrDefaultAsync(h => h.Id == id).ConfigureAwait(false);
    }
}
#nullable restore