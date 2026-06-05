namespace Repositories;

using Data;
using Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

#nullable enable

public class IssueCodeRepository(ApplicationDbContext context)
    : Repository<IssueCode>(context), IIssueCodeRepository
{
    public async Task<IssueCode?> GetByCodeAsync(string code)
    {
        return await this.DbSet
            .FirstOrDefaultAsync(ic => ic.Code == code)
            .ConfigureAwait(false);
    }
}