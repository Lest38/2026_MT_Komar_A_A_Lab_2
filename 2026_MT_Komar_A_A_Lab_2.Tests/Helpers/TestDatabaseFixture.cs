using Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace _2026_MT_Komar_A_A_Lab_2.Tests.Helpers;

public sealed class TestDatabaseFixture : IDisposable
{
    private readonly SqliteConnection connection;
    private readonly DbContextOptions<ApplicationDbContext> options;
    private bool disposed;

    public TestDatabaseFixture()
    {
        connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .EnableSensitiveDataLogging()
            .Options;

        using var context = CreateContext();
        context.Database.EnsureCreated();
    }

    public ApplicationDbContext CreateContext() => new(options);

    public void Dispose()
    {
        if (!disposed)
        {
            connection.Dispose();
            disposed = true;
        }
    }
}