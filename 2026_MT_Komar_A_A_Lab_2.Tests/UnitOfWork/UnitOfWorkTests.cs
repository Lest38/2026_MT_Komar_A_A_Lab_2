using Entities;
using Microsoft.EntityFrameworkCore;
using _2026_MT_Komar_A_A_Lab_2.Tests.Helpers;

namespace _2026_MT_Komar_A_A_Lab_2.Tests.UnitOfWork;

[TestFixture]
public class UnitOfWorkTests
{
    private TestDatabaseFixture _fixture;

    [SetUp]
    public void SetUp()
    {
        _fixture = new TestDatabaseFixture();
    }

    [TearDown]
    public void TearDown()
    {
        _fixture?.Dispose();
    }

    [Test]
    public async Task SaveChangesAsync_ShouldPersistChanges()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var unitOfWork = new UnitsOfWork.UnitOfWork(context);
        var project = new Project
        {
            Name = "UoW Test",
            FolderPath = @"C:\Test\UoW"
        };

        // Act
        await unitOfWork.Projects.AddAsync(project);
        var result = await unitOfWork.SaveChangesAsync();

        // Assert
        Assert.That(result, Is.EqualTo(1));
        var savedProject = await context.Projects.FirstOrDefaultAsync(p => p.Name == "UoW Test");
        Assert.That(savedProject, Is.Not.Null);
    }

    [Test]
    public async Task Transaction_ShouldCommitAllChanges()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var unitOfWork = new UnitsOfWork.UnitOfWork(context);

        var project = new Project { Name = "Transaction Test", FolderPath = @"C:\Test\Transaction" };
        var step = new PipelineStepExecution
        {
            StageTypeId = 1,
            Status = "Success",
            StartedAt = DateTime.Now,
            DurationMs = 1000,
            ExitCode = 0,
            TotalErrors = 0,
            TotalWarnings = 0
        };

        // Act
        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Projects.AddAsync(project);
        await unitOfWork.SaveChangesAsync();

        step.ProjectId = project.Id;
        await unitOfWork.PipelineStepExecutions.AddAsync(step);
        await unitOfWork.SaveChangesAsync();

        await unitOfWork.CommitTransactionAsync();

        // Assert
        var savedProject = await context.Projects.FindAsync(project.Id);
        var savedStep = await context.PipelineStepExecutions.FindAsync(step.Id);

        Assert.Multiple(() =>
        {
            Assert.That(savedProject, Is.Not.Null);
            Assert.That(savedStep, Is.Not.Null);
        });
        Assert.That(savedStep.ProjectId, Is.EqualTo(project.Id));
    }

    [Test]
    public async Task Transaction_ShouldRollbackOnError()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var unitOfWork = new UnitsOfWork.UnitOfWork(context);

        var project = new Project
        {
            Name = "Rollback Test",
            FolderPath = @"C:\Test\Rollback"
        };

        // Act
        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Projects.AddAsync(project);

        var invalidStep = new PipelineStepExecution
        {
            ProjectId = project.Id,
            StageTypeId = 99999,
            Status = "Success",
            StartedAt = DateTime.Now,
            DurationMs = 1000,
            ExitCode = 0,
            TotalErrors = 0,
            TotalWarnings = 0
        };

        await unitOfWork.PipelineStepExecutions.AddAsync(invalidStep);

        try
        {
            await unitOfWork.SaveChangesAsync();
            Assert.Fail("Expected exception was not thrown");
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
        }

        // Assert
        var savedProject = await context.Projects.FirstOrDefaultAsync(p => p.Name == "Rollback Test");
        Assert.That(savedProject, Is.Null, "Project should not exist after rollback");

        var savedStep = await context.PipelineStepExecutions.FirstOrDefaultAsync(s => s.StageTypeId == 99999);
        Assert.That(savedStep, Is.Null, "Step should not exist after rollback");
    }

    [Test]
    public void Repositories_ShouldBeInitialized()
    {
        using var context = _fixture.CreateContext();
        var unitOfWork = new UnitsOfWork.UnitOfWork(context);

        Assert.Multiple(() =>
        {
            Assert.That(unitOfWork.Projects, Is.Not.Null);
            Assert.That(unitOfWork.StageTypes, Is.Not.Null);
            Assert.That(unitOfWork.PipelineStepExecutions, Is.Not.Null);
            Assert.That(unitOfWork.IssueLogs, Is.Not.Null);
            Assert.That(unitOfWork.CpuModels, Is.Not.Null);
            Assert.That(unitOfWork.Hosts, Is.Not.Null);
            Assert.That(unitOfWork.PerformanceTests, Is.Not.Null);
            Assert.That(unitOfWork.ThreadSpeedMetrics, Is.Not.Null);
        });
    }

    [Test]
    public void Dispose_ShouldDisposeContextAndTransaction()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var unitOfWork = new UnitsOfWork.UnitOfWork(context);

        // Act & Assert
        Assert.DoesNotThrow(() => unitOfWork.Dispose());
    }
}