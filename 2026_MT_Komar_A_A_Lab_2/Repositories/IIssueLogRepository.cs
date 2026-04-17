namespace Repositories;

using Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IIssueLogRepository : IRepository<IssueLog>
{
    Task<IEnumerable<IssueLog>> GetByPipelineStepExecutionIdAsync(int pipelineStepExecutionId);

    Task<IEnumerable<IssueLog>> GetErrorsByPipelineStepExecutionIdAsync(int pipelineStepExecutionId);

    Task<IEnumerable<IssueLog>> GetWarningsByPipelineStepExecutionIdAsync(int pipelineStepExecutionId);
}