using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2026_MT_Komar_A_A_Lab_2.Tests.Entities
{
    [TestFixture]
    public class CpuModelStructureTests : EntityStructureTestBase<CpuModel>
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
                HasPublicProperty("CpuModelId", typeof(int));
                HasPublicProperty("ModelName", typeof(string));
                HasPublicProperty("PhysicalCoreCount", typeof(int));
                HasPublicProperty("LogicalThreadCount", typeof(int));
            });
        }

        [Test]
        public void ToLogString_ContainsModelName()
        {
            var entity = new CpuModel { CpuModelId = 1, ModelName = "i7-12700K", PhysicalCoreCount = 12, LogicalThreadCount = 20 };
            Assert.That(entity.ToLogString(), Does.Contain("i7-12700K"));
        }
    }
}
