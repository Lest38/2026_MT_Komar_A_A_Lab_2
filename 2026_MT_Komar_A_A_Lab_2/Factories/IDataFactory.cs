namespace Factories;

using Entities;
using System.Collections.Generic;

public interface IDataFactory
{
    Host CreateHost();

    IEnumerable<PerformanceTest> CreatePerformanceTests();
}