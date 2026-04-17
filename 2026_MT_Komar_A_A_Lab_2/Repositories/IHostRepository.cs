namespace Repositories;

using Entities;
using System.Threading.Tasks;

#nullable enable
public interface IHostRepository : IRepository<Host>
{
    Task<Host?> GetDefaultHostAsync();

    Task<Host?> GetWithCpuModelAsync(int id);
}
#nullable restore