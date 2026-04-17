namespace Repositories;

using Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable
public interface IPipelineStepExecutionRepository : IRepository<PipelineStepExecution>
{
    Task<IEnumerable<PipelineStepExecution>> GetByProjectIdAsync(int projectId);

    Task<IEnumerable<PipelineStepExecution>> GetByStageTypeIdAsync(int stageTypeId);

    Task<PipelineStepExecution?> GetWithLogsAsync(int id);
}
#nullable restore