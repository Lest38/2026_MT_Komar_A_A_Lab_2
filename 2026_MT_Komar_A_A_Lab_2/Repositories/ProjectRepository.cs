namespace Repositories;

using Data;
using Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class ProjectRepository(ApplicationDbContext context)
    : Repository<Project>(context), IProjectRepository
{
    public async Task<Project> GetByFolderPathAsync(string folderPath)
    {
        return await this.DbSet.FirstOrDefaultAsync(p => p.FolderPath == folderPath).ConfigureAwait(false);
    }

    public async Task<Project> GetWithPipelineStepsAsync(int id)
    {
        return await this.DbSet
            .Include(p => p.PipelineStepExecutions)
            .ThenInclude(pse => pse.StageType)
            .FirstOrDefaultAsync(p => p.Id == id).ConfigureAwait(false);
    }
}