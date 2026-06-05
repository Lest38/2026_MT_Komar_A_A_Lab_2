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
                HasPublicProperty("CpuModelId", typeof(int?));
                HasPublicProperty("RamGb", typeof(decimal));
                HasPublicProperty("OperatingSystemTypeId", typeof(int));
            });
        }

        [Test]
        public void ToLogString_ContainsOperatingSystem()
        {
            var osType = new OperatingSystemType { OperatingSystemTypeId = 1, Name = "Windows 11 Pro (64-bit)" };
            var entity = new Host
            {
                HostId = 3,
                CpuModelId = 1,
                RamGb = 32,
                OperatingSystemTypeId = 1,
                OperatingSystemType = osType,
            };
            Assert.That(entity.ToLogString(), Does.Contain("Windows 11"));
        }
    }
}