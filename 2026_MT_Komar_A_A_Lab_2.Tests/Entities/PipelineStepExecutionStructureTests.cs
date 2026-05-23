using Entities;

namespace _2026_MT_Komar_A_A_Lab_2.Tests.Entities;

[TestFixture]
public class PipelineStepExecutionStructureTests
    : EntityStructureTestBase<PipelineStepExecution>
{
    [Test]
    public void HasExpectedProperties()
    {
        Assert.Multiple(() =>
        {
            HasPublicProperty(nameof(PipelineStepExecution.ProjectId), typeof(int));
            HasPublicProperty(nameof(PipelineStepExecution.StageTypeId), typeof(int));
            HasPublicProperty(nameof(PipelineStepExecution.ExecutionStatusId), typeof(int)); // было Status
            HasPublicProperty(nameof(PipelineStepExecution.StartedAt), typeof(DateTime));
            HasPublicProperty(nameof(PipelineStepExecution.DurationMs), typeof(long));
            HasPublicProperty(nameof(PipelineStepExecution.TotalErrors), typeof(int));
            HasPublicProperty(nameof(PipelineStepExecution.TotalWarnings), typeof(int));
        });
    }

    [Test]
    public void ToLogString_ContainsStatusAndErrors()
    {
        var step = new PipelineStepExecution
        {
            ProjectId = 1,
            StageTypeId = 1,
            ExecutionStatusId = 3,
            ExecutionStatus = new ExecutionStatus { ExecutionStatusId = 3, Name = "Failed" },
            StartedAt = DateTime.UtcNow,
            DurationMs = 1000,
            TotalErrors = 3,
            TotalWarnings = 1,
        };

        var log = step.ToLogString();

        Assert.Multiple(() =>
        {
            Assert.That(log, Does.Contain("Failed"));
            Assert.That(log, Does.Contain("3"));
            Assert.That(log, Does.Contain("1000"));
        });
    }
}