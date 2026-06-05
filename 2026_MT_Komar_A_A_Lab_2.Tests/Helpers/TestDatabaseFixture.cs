using Data;
using Entities;
using Microsoft.EntityFrameworkCore;


namespace _2026_MT_Komar_A_A_Lab_2.Tests.Helpers;

public class TestDatabaseFixture : IDisposable
{
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private bool _disposed;

    public TestDatabaseFixture()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(_options);
        SeedLookupData(context);
    }

    public ApplicationDbContext CreateContext() => new(_options);

    private static void SeedLookupData(ApplicationDbContext context)
    {
        context.SeverityTypes.AddRange(
            new SeverityType { SeverityTypeId = 1, Name = "Error", Description = "Compilation or runtime error" },
            new SeverityType { SeverityTypeId = 2, Name = "Warning", Description = "Non-fatal issue" },
            new SeverityType { SeverityTypeId = 3, Name = "Info", Description = "Informational message" });

        context.ExecutionStatuses.AddRange(
            new ExecutionStatus { ExecutionStatusId = 1, Name = "Success", Description = "Step completed successfully" },
            new ExecutionStatus { ExecutionStatusId = 2, Name = "Failed", Description = "Step failed" },
            new ExecutionStatus { ExecutionStatusId = 3, Name = "Skipped", Description = "Step was skipped" },
            new ExecutionStatus { ExecutionStatusId = 4, Name = "Running", Description = "Step is in progress" });

        context.StageTypes.AddRange(
            new StageType { StageTypeId = 1, Name = "Build" },
            new StageType { StageTypeId = 2, Name = "Test" },
            new StageType { StageTypeId = 3, Name = "Clean" },
            new StageType { StageTypeId = 4, Name = "Run" });

        context.IssueCodes.AddRange(
            new IssueCode { IssueCodeId = 1, Code = "CS0246", Description = "The type or namespace name could not be found" },
            new IssueCode { IssueCodeId = 2, Code = "CS1001", Description = "Identifier expected" },
            new IssueCode { IssueCodeId = 3, Code = "CS0168", Description = "The variable is declared but never used" },
            new IssueCode { IssueCodeId = 4, Code = "CS0219", Description = "The variable is assigned but its value is never used" });

        context.SaveChanges();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}