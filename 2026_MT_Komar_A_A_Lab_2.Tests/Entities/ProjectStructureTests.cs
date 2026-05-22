using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2026_MT_Komar_A_A_Lab_2.Tests.Entities
{
    [TestFixture]
    public class ProjectStructureTests : EntityStructureTestBase<Project>
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
                HasPublicProperty("ProjectId", typeof(int));
                HasPublicProperty("Name", typeof(string));
                HasPublicProperty("FolderPath", typeof(string));
            });
        }

        [Test]
        public void ToLogString_ContainsClassNameAndId()
        {
            var entity = new Project { ProjectId = 42, Name = "Demo", FolderPath = @"C:\x" };
            var log = entity.ToLogString();
            Assert.Multiple(() =>
            {
                Assert.That(log, Does.Contain("Project"));
                Assert.That(log, Does.Contain("42"));
            });
        }
    }
}
