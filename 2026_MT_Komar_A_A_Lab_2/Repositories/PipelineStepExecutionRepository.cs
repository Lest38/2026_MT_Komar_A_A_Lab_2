namespace Repositories;

using Data;
using Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class PipelineStepExecutionRepository(ApplicationDbContext context)
    : Repository<PipelineStepExecution>(context), IPipelineStepExecutionRepository
{
    public async Task<IEnumerable<PipelineStepExecution>> GetByProjectIdAsync(int projectId)
    {
        return await this.DbSet
            .Where(pse => pse.ProjectId == projectId)
            .OrderByDescending(pse => pse.StartedAt)
            .ToListAsync().ConfigureAwait(false);
    }

    public async Task<IEnumerable<PipelineStepExecution>> GetByStageTypeIdAsync(int stageTypeId)
    {
        return await this.DbSet
            .Where(pse => pse.StageTypeId == stageTypeId)
            .OrderByDescending(pse => pse.StartedAt)
            .ToListAsync().ConfigureAwait(false);
    }

    public async Task<PipelineStepExecution> GetWithLogsAsync(int id)
    {
        return await this.DbSet
            .Include(pse => pse.IssueLogs)
            .FirstOrDefaultAsync(pse => pse.Id == id).ConfigureAwait(false);
    }
}