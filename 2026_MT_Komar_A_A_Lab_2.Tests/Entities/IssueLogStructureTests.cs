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
                HasPublicProperty("SeverityTypeId", typeof(int));
                HasPublicProperty("Message", typeof(string));
                HasPublicProperty("IssueCodeId", typeof(int?));
            });
        }

        [Test]
        public void ToLogString_ContainsSeverityAndCode()
        {
            var severityType = new SeverityType { SeverityTypeId = 1, Name = "Error" };
            var issueCode = new IssueCode { IssueCodeId = 1, Code = "CS0246" };

            var entity = new IssueLog
            {
                IssueLogId = 9,
                SeverityTypeId = 1,
                SeverityType = severityType,
                IssueCodeId = 1,
                IssueCode = issueCode,
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
