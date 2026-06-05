using System.ComponentModel.DataAnnotations;
using Entities;

namespace _2026_MT_Komar_A_A_Lab_2.Tests.Entities;

[TestFixture]
public class EntityValidationTests
{
    private static bool Validate(object model, out ICollection<ValidationResult> results)
    {
        var context = new ValidationContext(model, serviceProvider: null, items: null);
        results = [];
        return Validator.TryValidateObject(model, context, results, validateAllProperties: true);
    }

    private static bool HasErrorFor(object model, string propertyName)
    {
        Validate(model, out var results);
        return results.Any(r => r.MemberNames.Contains(propertyName));
    }

    [Test]
    public void Project_EmptyObject_FailsOnNameAndFolderPath()
    {
        var project = new Project();
        Assert.Multiple(() =>
        {
            Assert.That(Validate(project, out _), Is.False);
            Assert.That(HasErrorFor(project, nameof(Project.Name)), Is.True);
            Assert.That(HasErrorFor(project, nameof(Project.FolderPath)), Is.True);
        });
    }

    [Test]
    public void Project_NameExceedsMaxLength_Fails()
    {
        var project = new Project { Name = new string('X', 201), FolderPath = @"C:\ok" };
        Assert.That(HasErrorFor(project, nameof(Project.Name)), Is.True);
    }

    [Test]
    public void Project_ValidData_Passes()
    {
        var project = new Project { Name = "Valid Project", FolderPath = @"C:\Valid\Path" };
        Assert.Multiple(() =>
        {
            Assert.That(Validate(project, out var results), Is.True);
            Assert.That(results, Is.Empty);
        });
    }

    [Test]
    public void PipelineStepExecution_ValidData_Passes()
    {
        var step = new PipelineStepExecution
        {
            ProjectId = 1,
            StageTypeId = 1,
            ExecutionStatusId = 2,
            StartedAt = DateTime.UtcNow,
            DurationMs = 1000,
        };
        Assert.Multiple(() =>
        {
            Assert.That(Validate(step, out var results), Is.True);
            Assert.That(results, Is.Empty);
        });
    }

    [Test]
    public void IssueLog_ValidData_Passes()
    {
        var log = new IssueLog
        {
            PipelineStepExecutionId = 1,
            LoggedAt = DateTime.UtcNow,
            SeverityType = new SeverityType { Name = "Error" },
            IssueCode = new IssueCode { Code = "CS0246" },
            Message = "Type not found"
        };
        Assert.Multiple(() =>
        {
            Assert.That(Validate(log, out var results), Is.True);
            Assert.That(results, Is.Empty);
        });
    }

    [Test]
    public void CpuModel_EmptyModelName_Fails()
    {
        var cpu = new CpuModel { ModelName = string.Empty, PhysicalCoreCount = 8, LogicalThreadCount = 16 };
        Assert.That(HasErrorFor(cpu, nameof(CpuModel.ModelName)), Is.True);
    }

    [Test]
    public void CpuModel_ValidData_Passes()
    {
        var cpu = new CpuModel { ModelName = "Intel Core i7", PhysicalCoreCount = 8, LogicalThreadCount = 16 };
        Assert.Multiple(() =>
        {
            Assert.That(Validate(cpu, out var results), Is.True);
            Assert.That(results, Is.Empty);
        });
    }

    [Test]
    public void Host_OperatingSystemExceedsMaxLength_Fails()
    {
        var host = new Host { CpuModelId = 1, RamGb = 32m, OperatingSystem = new string('W', 201) };
        Assert.That(HasErrorFor(host, nameof(Host.OperatingSystem)), Is.True);
    }

    [Test]
    public void Host_ValidData_Passes()
    {
        var host = new Host { CpuModelId = 1, RamGb = 32m, OperatingSystem = "Windows 11" };
        Assert.Multiple(() =>
        {
            Assert.That(Validate(host, out var results), Is.True);
            Assert.That(results, Is.Empty);
        });
    }

    [Test]
    public void StageType_ValidData_Passes()
    {
        var st = new StageType { Name = "Build" };
        Assert.Multiple(() =>
        {
            Assert.That(Validate(st, out var results), Is.True);
            Assert.That(results, Is.Empty);
        });
    }
}