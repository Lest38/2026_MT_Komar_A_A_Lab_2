using System;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Entities;
using Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UnitsOfWork;

namespace DesignTimeDbContextFactory;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite("Data Source=app.db"));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        await dbContext.Database.MigrateAsync().ConfigureAwait(false);

        await ShowStatistics(unitOfWork).ConfigureAwait(false);
    }

    private static async Task ShowStatistics(IUnitOfWork unitOfWork)
    {
        var projects = await unitOfWork.Projects.GetAllAsync().ConfigureAwait(false);
        Console.WriteLine($"Projects: {projects.Count()}");

        var steps = await unitOfWork.PipelineStepExecutions.GetAllAsync().ConfigureAwait(false);
        Console.WriteLine($"Pipeline Steps: {steps.Count()}");

        var logs = await unitOfWork.IssueLogs.GetAllAsync().ConfigureAwait(false);
        Console.WriteLine($"Total Logs: {logs.Count()}");

        var metrics = await unitOfWork.ThreadSpeedMetrics.GetAllAsync().ConfigureAwait(false);
        Console.WriteLine($"Performance Metrics: {metrics.Count()}");
    }
}