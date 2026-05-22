using Entities;

namespace _2026_MT_Komar_A_A_Lab_2.Tests.Entities
{
    [TestFixture]
    public class HostStructureTests : EntityStructureTestBase<Host>
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
                HasPublicProperty("HostId", typeof(int));
                HasPublicProperty("CpuModelId", typeof(int));
                HasPublicProperty("RamGb", typeof(decimal));
                HasPublicProperty("OperatingSystem", typeof(string));
            });
        }

        [Test]
        public void ToLogString_ContainsOperatingSystem()
        {
            var entity = new Host { HostId = 3, CpuModelId = 1, RamGb = 32, OperatingSystem = "Windows 11" };
            Assert.That(entity.ToLogString(), Does.Contain("Windows 11"));
        }
    }
}
