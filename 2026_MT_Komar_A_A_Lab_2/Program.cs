using System;
using System.IO;
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
    private static async Task Main()
    {
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite("Data Source=app.db"));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDataFactory, DefaultDataFactory>();

        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        await dbContext.Database.MigrateAsync().ConfigureAwait(false);

        await InitializeData(unitOfWork).ConfigureAwait(false);

        await DemonstrateWorkflow(unitOfWork).ConfigureAwait(false);
    }

    private static async Task InitializeData(IUnitOfWork unitOfWork)
    {
        var projectFolder = Path.GetTempPath() + "TestProject";

        var existingProject = await unitOfWork.Projects
            .GetByFolderPathAsync(projectFolder)
            .ConfigureAwait(false);

        if (existingProject != null)
        {
            Console.WriteLine($"Project already exists (Id: {existingProject.Id})");
            Console.WriteLine($"   Path: {existingProject.FolderPath}");
            return;
        }

        var project = new Project
        {
            Name = "TestProject",
            FolderPath = projectFolder,
        };
        await unitOfWork.Projects.AddAsync(project).ConfigureAwait(false);
        await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        Console.WriteLine($"Project created (Id: {project.Id}) at: {projectFolder}");
    }

    // TODO: Implement a demonstration of the workflow that creates a project, adds stages, executes pipeline steps, and logs issues.
    private static async Task DemonstrateWorkflow(IUnitOfWork unitOfWork)
    {
        throw new NotImplementedException("Implement the workflow demonstration here.");
    }
}