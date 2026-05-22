using Entities;

namespace _2026_MT_Komar_A_A_Lab_2.Tests.Entities
{
    [TestFixture]
    public class StageTypeStructureTests : EntityStructureTestBase<StageType>
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
                HasPublicProperty("StageTypeId", typeof(int));
                HasPublicProperty("Name", typeof(string));
            });
        }

        [Test]
        public void ToLogString_ContainsClassNameAndId()
        {
            var entity = new StageType { StageTypeId = 7, Name = "Build" };
            var log = entity.ToLogString();
            Assert.Multiple(() =>
            {
                Assert.That(log, Does.Contain("StageType"));
                Assert.That(log, Does.Contain("7"));
            });
        }
    }
}
