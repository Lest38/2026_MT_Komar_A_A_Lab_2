using Entities;

namespace _2026_MT_Komar_A_A_Lab_2.Tests.Entities
{
    [TestFixture]
    public class PipelineStepExecutionStructureTests : EntityStructureTestBase<PipelineStepExecution>
    {
        [Test] public void InheritsBaseEntity() => EntityInheritsBaseEntity();
        [Test] public void IsPublicConcreteClass() => IsPublicEntity();
        [Test] public void ImplementsIEntityInt() => ImplementsIEntity();

        [Test]
        public void HasExpectedProperties()
        {
            Assert.Multiple(() =>
            {
                HasPublicReadOnlyProperty("Id", typeof(int));
                HasPublicProperty("PipelineStepExecutionId", typeof(int));
                HasPublicProperty("ProjectId", typeof(int));
                HasPublicProperty("StageTypeId", typeof(int));
                HasPublicProperty("Status", typeof(string));
                HasPublicProperty("StartedAt", typeof(DateTime));
                HasPublicProperty("DurationMs", typeof(long));
                HasPublicProperty("TotalErrors", typeof(int));
                HasPublicProperty("TotalWarnings", typeof(int));
            });
        }

        [Test]
        public void ToLogString_ContainsStatusAndErrors()
        {
            var entity = new PipelineStepExecution
            {
                PipelineStepExecutionId = 5,
                Status = "Failed",
                TotalErrors = 3,
                TotalWarnings = 1,
                StartedAt = DateTime.UtcNow,
                DurationMs = 1000
            };
            var log = entity.ToLogString();
            Assert.Multiple(() =>
            {
                Assert.That(log, Does.Contain("Failed"));
                Assert.That(log, Does.Contain("3"));
            });
        }
    }
}
