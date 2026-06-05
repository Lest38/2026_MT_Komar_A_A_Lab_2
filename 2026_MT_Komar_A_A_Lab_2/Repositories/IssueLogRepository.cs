namespace Repositories;

using Data;
using Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class IssueLogRepository(ApplicationDbContext context)
    : Repository<IssueLog>(context), IIssueLogRepository
{
    public async Task<IEnumerable<IssueLog>> GetByPipelineStepExecutionIdAsync(int pipelineStepExecutionId)
    {
        return await this.DbSet
            .Where(il => il.PipelineStepExecutionId == pipelineStepExecutionId)
            .OrderBy(il => il.LoggedAt)
            .ToListAsync().ConfigureAwait(false);
    }

    public async Task<IEnumerable<IssueLog>> GetErrorsByPipelineStepExecutionIdAsync(int pipelineStepExecutionId)
    {
        return await this.DbSet
            .Where(il => il.PipelineStepExecutionId == pipelineStepExecutionId
                      && il.SeverityType.Name == "Error")
            .OrderBy(il => il.LoggedAt)
            .ToListAsync().ConfigureAwait(false);
    }

    public async Task<IEnumerable<IssueLog>> GetWarningsByPipelineStepExecutionIdAsync(int pipelineStepExecutionId)
    {
        return await this.DbSet
            .Where(il => il.PipelineStepExecutionId == pipelineStepExecutionId
                      && il.SeverityType.Name == "Warning")
            .OrderBy(il => il.LoggedAt)
            .ToListAsync().ConfigureAwait(false);
    }
}
