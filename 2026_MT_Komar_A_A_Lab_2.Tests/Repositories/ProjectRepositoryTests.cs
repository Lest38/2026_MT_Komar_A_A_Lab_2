using Entities;
using Repositories;
using _2026_MT_Komar_A_A_Lab_2.Tests.Helpers;

namespace _2026_MT_Komar_A_A_Lab_2.Tests.Repositories;

[TestFixture]
public class ProjectRepositoryTests
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
    public async Task GetByFolderPathAsync_WhenProjectExists_ShouldReturnProject()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new ProjectRepository(context);
        var expectedPath = @"C:\Test\UniquePath";

        var project = new Project
        {
            Name = "Folder Path Test",
            FolderPath = expectedPath
        };
        await repository.AddAsync(project);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByFolderPathAsync(expectedPath);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.FolderPath, Is.EqualTo(expectedPath));
    }

    [Test]
    public async Task GetByFolderPathAsync_WhenProjectDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new ProjectRepository(context);

        // Act
        var result = await repository.GetByFolderPathAsync(@"C:\NonExistent\Path");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetWithPipelineStepsAsync_ShouldIncludeRelatedSteps()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var projectRepository = new ProjectRepository(context);
        var stepRepository = new Repository<PipelineStepExecution>(context);

        var project = new Project
        {
            Name = "Include Test",
            FolderPath = @"C:\Test\Include"
        };
        await projectRepository.AddAsync(project);
        await context.SaveChangesAsync();

        var step = new PipelineStepExecution
        {
            ProjectId = project.Id,
            StageTypeId = 1,
            Status = "Success",
            StartedAt = System.DateTime.Now,
            DurationMs = 1000,
            ExitCode = 0,
            TotalErrors = 0,
            TotalWarnings = 0
        };
        await stepRepository.AddAsync(step);
        await context.SaveChangesAsync();

        // Act
        var result = await projectRepository.GetWithPipelineStepsAsync(project.Id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.PipelineStepExecutions, Is.Not.Empty);
        Assert.That(result.PipelineStepExecutions.First().Id, Is.EqualTo(step.Id));
    }
}