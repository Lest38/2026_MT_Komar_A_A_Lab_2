using Data;
using Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace _2026_MT_Komar_A_A_Lab_2.Tests.Helpers;

public class TestDatabaseFixture : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private bool _disposed;

    public TestDatabaseFixture()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .EnableSensitiveDataLogging()
            .Options;

        using var context = CreateContext();
        context.Database.EnsureCreated();

        SeedData(context);
    }

    public ApplicationDbContext CreateContext()
    {
        return new ApplicationDbContext(_options);
    }

    private static void SeedData(ApplicationDbContext context)
    {
        if (!context.StageTypes.Any())
        {
            context.StageTypes.AddRange(
                new StageType { Id = 1, Name = "Build" },
                new StageType { Id = 2, Name = "Test" },
                new StageType { Id = 3, Name = "Clean" },
                new StageType { Id = 4, Name = "Run" }
            );
            context.SaveChanges();
        }

        if (!context.CpuModels.Any())
        {
            context.CpuModels.AddRange(
                new CpuModel { Id = 1, ModelName = "Intel Core i7-12700K", PhysicalCoreCount = 12, LogicalThreadCount = 20 },
                new CpuModel { Id = 2, ModelName = "AMD Ryzen 9 5900X", PhysicalCoreCount = 12, LogicalThreadCount = 24 }
            );
            context.SaveChanges();
        }

        if (!context.Hosts.Any())
        {
            context.Hosts.Add(new Host
            {
                Id = 1,
                CpuModelId = 1,
                RamGb = 32.00m,
                OperatingSystem = "Windows 11 Pro"
            });
            context.SaveChanges();
        }

        if (!context.PerformanceTests.Any())
        {
            context.PerformanceTests.Add(new PerformanceTest
            {
                Id = 1,
                Description = "Test Performance Description"
            });
            context.SaveChanges();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _connection?.Dispose();
        }
        _disposed = true;
    }
}