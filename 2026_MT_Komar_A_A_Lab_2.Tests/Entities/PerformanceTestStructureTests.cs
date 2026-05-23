using Entities;

namespace _2026_MT_Komar_A_A_Lab_2.Tests.Entities
{
    [TestFixture]
    public class PerformanceTestStructureTests : EntityStructureTestBase<PerformanceTest>
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
                HasPublicProperty("PerformanceTestId", typeof(int));
                HasPublicProperty("Description", typeof(string));
            });
        }

        [Test]
        public void ToLogString_ContainsDescription()
        {
            var entity = new PerformanceTest { PerformanceTestId = 2, Description = "Matrix 2000x2000" };
            Assert.That(entity.ToLogString(), Does.Contain("Matrix 2000x2000"));
        }
    }
}
