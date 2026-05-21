using Entities;
using Repositories;
using _2026_MT_Komar_A_A_Lab_2.Tests.Helpers;

namespace _2026_MT_Komar_A_A_Lab_2.Tests.Repositories;

[TestFixture]
public class PipelineStepExecutionRepositoryTests
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
    public async Task GetByProjectIdAsync_ShouldReturnStepsForSpecificProject()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var stepRepository = new PipelineStepExecutionRepository(context);
        var projectRepository = new ProjectRepository(context);

        var project1 = new Project { Name = "Project 1", FolderPath = @"C:\Test\1" };
        var project2 = new Project { Name = "Project 2", FolderPath = @"C:\Test\2" };

        await projectRepository.AddAsync(project1);
        await projectRepository.AddAsync(project2);
        await context.SaveChangesAsync();

        var step1 = new PipelineStepExecution
        {
            ProjectId = project1.Id,
            StageTypeId = 1,
            Status = "Success",
            StartedAt = DateTime.Now,
            DurationMs = 1000,
            ExitCode = 0,
            TotalErrors = 0,
            TotalWarnings = 0
        };

        var step2 = new PipelineStepExecution
        {
            ProjectId = project1.Id,
            StageTypeId = 2,
            Status = "Success",
            StartedAt = DateTime.Now,
            DurationMs = 2000,
            ExitCode = 0,
            TotalErrors = 0,
            TotalWarnings = 0
        };

        var step3 = new PipelineStepExecution
        {
            ProjectId = project2.Id,
            StageTypeId = 1,
            Status = "Failed",
            StartedAt = DateTime.Now,
            DurationMs = 500,
            ExitCode = 1,
            TotalErrors = 2,
            TotalWarnings = 1
        };

        await stepRepository.AddAsync(step1);
        await stepRepository.AddAsync(step2);
        await stepRepository.AddAsync(step3);
        await context.SaveChangesAsync();

        // Act
        var result = await stepRepository.GetByProjectIdAsync(project1.Id);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(s => s.ProjectId == project1.Id));
        });
    }

    [Test]
    public async Task GetByStageTypeIdAsync_ShouldReturnStepsForSpecificStage()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var stepRepository = new PipelineStepExecutionRepository(context);
        var projectRepository = new ProjectRepository(context);

        var project = new Project { Name = "Test Project", FolderPath = @"C:\Test\Stage" };
        await projectRepository.AddAsync(project);
        await context.SaveChangesAsync();

        var buildStep = new PipelineStepExecution
        {
            ProjectId = project.Id,
            StageTypeId = 1,
            Status = "Success",
            StartedAt = DateTime.Now,
            DurationMs = 1000,
            ExitCode = 0,
            TotalErrors = 0,
            TotalWarnings = 0
        };

        var testStep = new PipelineStepExecution
        {
            ProjectId = project.Id,
            StageTypeId = 2,
            Status = "Success",
            StartedAt = DateTime.Now,
            DurationMs = 2000,
            ExitCode = 0,
            TotalErrors = 0,
            TotalWarnings = 0
        };

        await stepRepository.AddAsync(buildStep);
        await stepRepository.AddAsync(testStep);
        await context.SaveChangesAsync();

        // Act
        var result = await stepRepository.GetByStageTypeIdAsync(1);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().StageTypeId, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task GetWithLogsAsync_ShouldIncludeRelatedIssueLogs()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var stepRepository = new PipelineStepExecutionRepository(context);
        var logRepository = new IssueLogRepository(context);
        var projectRepository = new ProjectRepository(context);

        var project = new Project { Name = "Log Test", FolderPath = @"C:\Test\Logs" };
        await projectRepository.AddAsync(project);
        await context.SaveChangesAsync();

        var step = new PipelineStepExecution
        {
            ProjectId = project.Id,
            StageTypeId = 1,
            Status = "Success",
            StartedAt = DateTime.Now,
            DurationMs = 1000,
            ExitCode = 0,
            TotalErrors = 2,
            TotalWarnings = 1
        };
        await stepRepository.AddAsync(step);
        await context.SaveChangesAsync();

        var log1 = new IssueLog
        {
            PipelineStepExecutionId = step.Id,
            LoggedAt = DateTime.Now,
            Severity = "Error",
            Code = "CS1001",
            Message = "Test error message"
        };

        var log2 = new IssueLog
        {
            PipelineStepExecutionId = step.Id,
            LoggedAt = DateTime.Now,
            Severity = "Warning",
            Code = "CS0168",
            Message = "Test warning message"
        };

        await logRepository.AddAsync(log1);
        await logRepository.AddAsync(log2);
        await context.SaveChangesAsync();

        // Act
        var result = await stepRepository.GetWithLogsAsync(step.Id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IssueLogs, Is.Not.Empty);
        Assert.That(result.IssueLogs, Has.Count.EqualTo(2));
    }
}