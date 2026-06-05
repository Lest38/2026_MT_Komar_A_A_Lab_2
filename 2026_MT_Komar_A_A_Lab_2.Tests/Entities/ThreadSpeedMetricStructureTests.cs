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
                HasPublicProperty(nameof(ThreadSpeedMetric.PerformanceTestId), typeof(int));
                HasPublicProperty(nameof(ThreadSpeedMetric.HostId), typeof(int));
                HasPublicProperty(nameof(ThreadSpeedMetric.PipelineStepExecutionId), typeof(int));
                HasPublicProperty(nameof(ThreadSpeedMetric.SequentialTimeMs), typeof(long));
                HasPublicProperty(nameof(ThreadSpeedMetric.ParallelTimeMs), typeof(long));
                HasPublicProperty(nameof(ThreadSpeedMetric.StartedAt), typeof(DateTime));
                HasPublicReadOnlyProperty(nameof(ThreadSpeedMetric.DurationMs), typeof(long));
                HasPublicReadOnlyProperty(nameof(ThreadSpeedMetric.EfficiencyCoefficient), typeof(decimal));
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
                StartedAt = DateTime.UtcNow,
            };
            var log = entity.ToLogString();
            Assert.Multiple(() =>
            {
                Assert.That(log, Does.Contain("8000"));
                Assert.That(log, Does.Contain("1000"));
            });
        }

        [Test]
        public void DurationMs_IsComputedFromSeqAndPar()
        {
            var entity = new ThreadSpeedMetric
            {
                SequentialTimeMs = 8_240,
                ParallelTimeMs = 1_340,
            };
            Assert.That(entity.DurationMs, Is.EqualTo(9_580));
        }
    }
}