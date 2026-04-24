namespace Repositories;

using Entities;
using System.Threading.Tasks;

public interface IPerformanceTestRepository : IRepository<PerformanceTest>
{
    Task<PerformanceTest> GetByDescriptionAsync(string description);
}