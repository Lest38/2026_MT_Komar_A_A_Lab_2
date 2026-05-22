namespace Factories;

using Entities;
using System.Collections.Generic;

public class DefaultDataFactory : IDataFactory
{
    public Host CreateHost() =>
        new
        ()
        {
            CpuModelId = 1,
            RamGb = 32.00m,
            OperatingSystem = "Windows 11 Pro (64-bit)",
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