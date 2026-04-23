namespace Factories;

using Entities;
using System.Collections.Generic;

public class DefaultDataFactory : IDataFactory
{
    public IEnumerable<PerformanceTest> CreatePerformanceTests()
    {
        return
        [
            new PerformanceTest { Description = "Matrix Multiplication 2000x2000" },
            new PerformanceTest { Description = "Fibonacci Calculation (n=45) - Recursive" },
            new PerformanceTest { Description = "Array Sorting (10,000,000 elements)" },
            new PerformanceTest { Description = "HTTP Request Simulation - 1000 requests" },
            new PerformanceTest { Description = "Database Query Performance - 10000 records" },
        ];
    }

    public IEnumerable<CpuModel> CreateCpuModels()
    {
        return
        [
            new CpuModel { ModelName = "Intel Core i7-12700K", PhysicalCoreCount = 12, LogicalThreadCount = 20 },
            new CpuModel { ModelName = "AMD Ryzen 9 5900X", PhysicalCoreCount = 12, LogicalThreadCount = 24 },
            new CpuModel { ModelName = "Intel Core i9-13900K", PhysicalCoreCount = 24, LogicalThreadCount = 32 },
        ];
    }

    public Host CreateHost()
    {
        return new Host
        {
            CpuModelId = 1,
            RamGb = 32.00m,
            OperatingSystem = "Windows 11 Pro (64-bit)",
        };
    }
}