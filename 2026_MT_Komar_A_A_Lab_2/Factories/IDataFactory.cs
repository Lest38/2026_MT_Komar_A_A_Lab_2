namespace Factories;

using Entities;
using System.Collections.Generic;

public interface IDataFactory
{
    IEnumerable<OperatingSystemType> CreateOperatingSystemTypes();

    IEnumerable<CpuModel> CreateCpuModels();

    IEnumerable<SeverityType> CreateSeverityTypes();

    IEnumerable<ExecutionStatus> CreateExecutionStatuses();

    IEnumerable<StageType> CreateStageTypes();

    IEnumerable<IssueCode> CreateIssueCodes();

    Host CreateHost();

    IEnumerable<PerformanceTest> CreatePerformanceTests();
}