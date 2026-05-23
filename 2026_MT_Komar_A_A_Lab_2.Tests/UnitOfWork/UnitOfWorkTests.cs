using Entities;
using Microsoft.EntityFrameworkCore;
using _2026_MT_Komar_A_A_Lab_2.Tests.Helpers;

namespace _2026_MT_Komar_A_A_Lab_2.Tests.UnitOfWork;

[TestFixture]
public class UnitOfWorkTests
{
    private TestDatabaseFixture fixture = null!;

    [SetUp] public void SetUp() => fixture = new TestDatabaseFixture();
    [TearDown] public void TearDown() => fixture.Dispose();

    [Test]
    public void AllRepositories_AreNotNull_AfterConstruction()
    {
        using var context = fixture.CreateContext();
        using var unitOfWork = new global::UnitsOfWork.UnitOfWork(context);

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
    public async Task Transaction_Commit_PersistsBothEntities()
    {
        await using var context = fixture.CreateContext();
        using var unitOfWork = new global::UnitsOfWork.UnitOfWork(context);

        await unitOfWork.BeginTransactionAsync();

        var project = new Project { Name = "Tx Commit", FolderPath = @"C:\Tx\Commit" };
        await unitOfWork.Projects.AddAsync(project);
        await unitOfWork.SaveChangesAsync();

        var step = new PipelineStepExecution
        {
            ProjectId = project.Id,
            StageTypeId = 1,
            ExecutionStatusId = 2,
            StartedAt = DateTime.UtcNow,
            DurationMs = 2000,
        };
        await unitOfWork.PipelineStepExecutions.AddAsync(step);
        await unitOfWork.SaveChangesAsync();

        await unitOfWork.CommitTransactionAsync();

        var savedProject = await context.Projects.FindAsync(project.Id);
        var savedStep = await context.PipelineStepExecutions.FindAsync(step.Id);

        Assert.Multiple(() =>
        {
            Assert.That(savedProject, Is.Not.Null);
            Assert.That(savedStep, Is.Not.Null);
            Assert.That(savedStep!.ProjectId, Is.EqualTo(project.Id));
        });
    }

    [Test]
    public async Task Transaction_Rollback_NothingIsPersisted()
    {
        await using var context = fixture.CreateContext();
        using var unitOfWork = new global::UnitsOfWork.UnitOfWork(context);

        await unitOfWork.BeginTransactionAsync();

        var project = new Project { Name = "Tx Rollback", FolderPath = @"C:\Tx\Rollback" };
        await unitOfWork.Projects.AddAsync(project);

        var invalidStep = new PipelineStepExecution
        {
            ProjectId = project.Id,
            StageTypeId = 99999,
            ExecutionStatusId = 2,
            StartedAt = DateTime.UtcNow,
            DurationMs = 100,
        };
        await unitOfWork.PipelineStepExecutions.AddAsync(invalidStep);

        try
        {
            await unitOfWork.SaveChangesAsync();
            Assert.Fail("Expected an exception from invalid FK");
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
        }

        var projectInDb = await context.Projects
            .FirstOrDefaultAsync(p => p.Name == "Tx Rollback");
        Assert.That(projectInDb, Is.Null, "Nothing should be persisted after rollback");
    }

    [Test]
    public void Dispose_CalledOnce_DoesNotThrow()
    {
        using var context = fixture.CreateContext();
        var unitOfWork = new global::UnitsOfWork.UnitOfWork(context);

        Assert.DoesNotThrow(() => unitOfWork.Dispose());
    }
}