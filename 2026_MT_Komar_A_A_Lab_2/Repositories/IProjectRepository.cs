#nullable enable

namespace Repositories;

using Entities;
using System.Threading.Tasks;

public interface IProjectRepository : IRepository<Project>
{
    Task<Project?> GetByFolderPathAsync(string folderPath);

    Task<Project?> GetWithPipelineStepsAsync(int id);
}
#nullable restore