namespace Factories;

using Entities;
using System.Collections.Generic;

public class DefaultDataFactory : IDataFactory
{
    public IEnumerable<OperatingSystemType> CreateOperatingSystemTypes() =>
    [
        new OperatingSystemType { OperatingSystemTypeId = 1, Name = "Windows 11 Pro (64-bit)" },
        new OperatingSystemType { OperatingSystemTypeId = 2, Name = "Windows 10 Pro (64-bit)" },
        new OperatingSystemType { OperatingSystemTypeId = 3, Name = "Ubuntu 24.04 LTS (64-bit)" },
        new OperatingSystemType { OperatingSystemTypeId = 4, Name = "macOS Sequoia 15" },
    ];

    public IEnumerable<CpuModel> CreateCpuModels() =>
    [
        new CpuModel { CpuModelId = 1, ModelName = "AMD Ryzen 9 7950X",    PhysicalCoreCount = 16, LogicalThreadCount = 32 },
        new CpuModel { CpuModelId = 2, ModelName = "Intel Core i9-13900K", PhysicalCoreCount = 24, LogicalThreadCount = 32 },
        new CpuModel { CpuModelId = 3, ModelName = "AMD Ryzen 5 5600X",    PhysicalCoreCount = 6,  LogicalThreadCount = 12 },
    ];

    public IEnumerable<SeverityType> CreateSeverityTypes() =>
    [
        new SeverityType { SeverityTypeId = 1, Name = "Error",   Description = "Compilation or runtime error" },
        new SeverityType { SeverityTypeId = 2, Name = "Warning", Description = "Non-fatal issue" },
        new SeverityType { SeverityTypeId = 3, Name = "Info",    Description = "Informational message" },
    ];

    public IEnumerable<ExecutionStatus> CreateExecutionStatuses() =>
    [
        new ExecutionStatus { ExecutionStatusId = 1, Name = "Success", Description = "Step completed successfully" },
        new ExecutionStatus { ExecutionStatusId = 2, Name = "Failed",  Description = "Step failed" },
        new ExecutionStatus { ExecutionStatusId = 3, Name = "Skipped", Description = "Step was skipped" },
        new ExecutionStatus { ExecutionStatusId = 4, Name = "Running", Description = "Step is in progress" },
    ];

    public IEnumerable<StageType> CreateStageTypes() =>
    [
        new StageType { StageTypeId = 1, Name = "Build" },
        new StageType { StageTypeId = 2, Name = "Test" },
        new StageType { StageTypeId = 3, Name = "Clean" },
        new StageType { StageTypeId = 4, Name = "Run" },
    ];

    public IEnumerable<IssueCode> CreateIssueCodes() =>
    [
        new IssueCode { IssueCodeId = 1, Code = "CS0246", Description = "The type or namespace name could not be found" },
        new IssueCode { IssueCodeId = 2, Code = "CS0103", Description = "The name does not exist in the current context" },
        new IssueCode { IssueCodeId = 3, Code = "CS1001", Description = "Identifier expected" },
        new IssueCode { IssueCodeId = 4, Code = "CS0168", Description = "The variable is declared but never used" },
        new IssueCode { IssueCodeId = 5, Code = "CS0219", Description = "The variable is assigned but its value is never used" },
    ];

    public Host CreateHost() =>
        new
        ()
        {
            CpuModelId = 1,
            RamGb = 32.00m,
            OperatingSystemTypeId = 1,
        };

    public IEnumerable<PerformanceTest> CreatePerformanceTests() =>
    [
        new PerformanceTest { Description = "Matrix Multiplication 2000x2000" },
        new PerformanceTest { Description = "Fibonacci Calculation (n=45) - Recursive" },
        new PerformanceTest { Description = "Array Sorting (10,000,000 elements)" },
        new PerformanceTest { Description = "HTTP Request Simulation - 1000 requests" },
        new PerformanceTest { Description = "Database Query Performance - 10000 records" },
    ];
}