using Entities;

namespace _2026_MT_Komar_A_A_Lab_2.Tests.Entities
{
    [TestFixture]
    public class ThreadSpeedMetricStructureTests : EntityStructureTestBase<ThreadSpeedMetric>
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
                HasPublicProperty("ThreadSpeedMetricId", typeof(int));
                HasPublicProperty("PerformanceTestId", typeof(int));
                HasPublicProperty("HostId", typeof(int));
                HasPublicProperty("PipelineStepExecutionId", typeof(int));
                HasPublicProperty("SequentialTimeMs", typeof(long));
                HasPublicProperty("ParallelTimeMs", typeof(long));
                HasPublicProperty("EfficiencyCoefficient", typeof(decimal));
                HasPublicProperty("StartedAt", typeof(DateTime));
                HasPublicProperty("DurationMs", typeof(long));
            });
        }

        [Test]
        public void ToLogString_ContainsTimes()
        {
            var entity = new ThreadSpeedMetric
            {
                ThreadSpeedMetricId = 1,
                SequentialTimeMs = 8000,
                ParallelTimeMs = 1000,
                EfficiencyCoefficient = 8.0m,
                StartedAt = DateTime.UtcNow,
                DurationMs = 9000
            };
            var log = entity.ToLogString();
            Assert.Multiple(() =>
            {
                Assert.That(log, Does.Contain("8000"));
                Assert.That(log, Does.Contain("1000"));
            });
        }
    }
}
