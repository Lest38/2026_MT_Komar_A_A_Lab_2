namespace Repositories;

using Entities;
using System.Threading.Tasks;

#nullable enable
public interface IPerformanceTestRepository : IRepository<PerformanceTest>
{
    Task<PerformanceTest?> GetByDescriptionAsync(string description);
}
#nullable restore