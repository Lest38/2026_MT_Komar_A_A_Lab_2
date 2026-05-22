using Entities;

namespace _2026_MT_Komar_A_A_Lab_2.Tests.Entities
{
    [TestFixture]
    public class IssueLogStructureTests : EntityStructureTestBase<IssueLog>
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
                HasPublicProperty("IssueLogId", typeof(int));
                HasPublicProperty("PipelineStepExecutionId", typeof(int));
                HasPublicProperty("LoggedAt", typeof(DateTime));
                HasPublicProperty("Severity", typeof(string));
                HasPublicProperty("Message", typeof(string));
            });
        }

        [Test]
        public void ToLogString_ContainsSeverityAndCode()
        {
            var entity = new IssueLog
            {
                IssueLogId = 9,
                Severity = "Error",
                Code = "CS0246",
                Message = "Type not found",
                LoggedAt = DateTime.UtcNow
            };
            var log = entity.ToLogString();
            Assert.Multiple(() =>
            {
                Assert.That(log, Does.Contain("Error"));
                Assert.That(log, Does.Contain("CS0246"));
            });
        }
    }
}
