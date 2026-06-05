namespace Repositories;

using Entities;
using System.Threading.Tasks;

#nullable enable

public interface IIssueCodeRepository : IRepository<IssueCode>
{
    Task<IssueCode?> GetByCodeAsync(string code);
}