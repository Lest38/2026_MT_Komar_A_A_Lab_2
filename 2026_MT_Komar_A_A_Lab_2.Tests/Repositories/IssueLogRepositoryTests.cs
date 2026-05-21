using Entities;
using Repositories;
using _2026_MT_Komar_A_A_Lab_2.Tests.Helpers;

namespace _2026_MT_Komar_A_A_Lab_2.Tests.Repositories;

[TestFixture]
public class IssueLogRepositoryTests
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
    public async Task GetByPipelineStepExecutionIdAsync_ShouldReturnLogsForSpecificStep()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var logRepository = new IssueLogRepository(context);
        var stepRepository = new PipelineStepExecutionRepository(context);
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
            Message = "Error 1"
        };

        var log2 = new IssueLog
        {
            PipelineStepExecutionId = step.Id,
            LoggedAt = DateTime.Now,
            Severity = "Warning",
            Code = "CS0168",
            Message = "Warning 1"
        };

        await logRepository.AddAsync(log1);
        await logRepository.AddAsync(log2);
        await context.SaveChangesAsync();

        // Act
        var result = await logRepository.GetByPipelineStepExecutionIdAsync(step.Id);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(l => l.PipelineStepExecutionId == step.Id));
        });
    }

    [Test]
    public async Task GetErrorsByPipelineStepExecutionIdAsync_ShouldReturnOnlyErrors()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var logRepository = new IssueLogRepository(context);
        var stepRepository = new PipelineStepExecutionRepository(context);
        var projectRepository = new ProjectRepository(context);

        var project = new Project { Name = "Error Test", FolderPath = @"C:\Test\Errors" };
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

        var error = new IssueLog
        {
            PipelineStepExecutionId = step.Id,
            LoggedAt = DateTime.Now,
            Severity = "Error",
            Code = "CS1001",
            Message = "Error message"
        };

        var warning = new IssueLog
        {
            PipelineStepExecutionId = step.Id,
            LoggedAt = DateTime.Now,
            Severity = "Warning",
            Code = "CS0168",
            Message = "Warning message"
        };

        await logRepository.AddAsync(error);
        await logRepository.AddAsync(warning);
        await context.SaveChangesAsync();

        // Act
        var result = await logRepository.GetErrorsByPipelineStepExecutionIdAsync(step.Id);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.All(l => l.Severity == "Error"));
        });
    }

    [Test]
    public async Task GetWarningsByPipelineStepExecutionIdAsync_ShouldReturnOnlyWarnings()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var logRepository = new IssueLogRepository(context);
        var stepRepository = new PipelineStepExecutionRepository(context);
        var projectRepository = new ProjectRepository(context);

        var project = new Project { Name = "Warning Test", FolderPath = @"C:\Test\Warnings" };
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
            TotalErrors = 0,
            TotalWarnings = 2
        };
        await stepRepository.AddAsync(step);
        await context.SaveChangesAsync();

        var warning1 = new IssueLog
        {
            PipelineStepExecutionId = step.Id,
            LoggedAt = DateTime.Now,
            Severity = "Warning",
            Code = "CS0168",
            Message = "Warning 1"
        };

        var warning2 = new IssueLog
        {
            PipelineStepExecutionId = step.Id,
            LoggedAt = DateTime.Now,
            Severity = "Warning",
            Code = "CS0219",
            Message = "Warning 2"
        };

        await logRepository.AddAsync(warning1);
        await logRepository.AddAsync(warning2);
        await context.SaveChangesAsync();

        // Act
        var result = await logRepository.GetWarningsByPipelineStepExecutionIdAsync(step.Id);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(l => l.Severity == "Warning"));
        });
    }
}