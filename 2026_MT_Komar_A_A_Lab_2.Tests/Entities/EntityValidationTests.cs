using System.ComponentModel.DataAnnotations;
using Entities;

namespace _2026_MT_Komar_A_A_Lab_2.Tests.Entities;

[TestFixture]
public class EntityValidationTests
{
    private static bool ValidateModel(object model, out ICollection<ValidationResult> results)
    {
        var context = new ValidationContext(model, null, null);
        results = [];
        return Validator.TryValidateObject(model, context, results, true);
    }

    private static bool HasValidationErrorFor(object model, string propertyName)
    {
        ValidateModel(model, out var results);
        return results.Any(r => r.MemberNames.Contains(propertyName));
    }

    [Test]
    public void Project_WithoutRequiredFields_ShouldBeInvalid()
    {
        // Arrange
        var project = new Project();

        // Act
        var isValid = ValidateModel(project, out _);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(HasValidationErrorFor(project, "Name"), Is.True);
            Assert.That(HasValidationErrorFor(project, "FolderPath"), Is.True);
        });
    }

    [Test]
    public void Project_WithValidData_ShouldBeValid()
    {
        // Arrange
        var project = new Project
        {
            Name = "Valid Project",
            FolderPath = @"C:\Valid\Path"
        };

        // Act
        var isValid = ValidateModel(project, out var results);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(results, Is.Empty);
        });
    }

    [Test]
    public void Project_NameExceedsMaxLength_ShouldBeInvalid()
    {
        // Arrange
        var project = new Project
        {
            Name = new string('A', 201),
            FolderPath = @"C:\Valid\Path"
        };

        // Act
        var isValid = ValidateModel(project, out _);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(HasValidationErrorFor(project, "Name"), Is.True);
        });
    }

    [Test]
    public void PipelineStepExecution_WithoutRequiredFields_ShouldBeInvalid()
    {
        // Arrange
        var step = new PipelineStepExecution();

        // Act
        var isValid = ValidateModel(step, out _);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(HasValidationErrorFor(step, "Status"), Is.True);
        });
    }

    [Test]
    public void PipelineStepExecution_WithEmptyStatus_ShouldBeInvalid()
    {
        // Arrange
        var step = new PipelineStepExecution
        {
            ProjectId = 1,
            StageTypeId = 1,
            Status = string.Empty,
            StartedAt = DateTime.Now,
            DurationMs = 1000
        };

        // Act
        var isValid = ValidateModel(step, out _);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(HasValidationErrorFor(step, "Status"), Is.True);
        });
    }

    [Test]
    public void PipelineStepExecution_WithStatusExceedingMaxLength_ShouldBeInvalid()
    {
        // Arrange
        var step = new PipelineStepExecution
        {
            ProjectId = 1,
            StageTypeId = 1,
            Status = new string('A', 21),
            StartedAt = DateTime.Now,
            DurationMs = 1000
        };

        // Act
        var isValid = ValidateModel(step, out _);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(HasValidationErrorFor(step, "Status"), Is.True);
        });
    }

    [Test]
    public void PipelineStepExecution_WithValidData_ShouldBeValid()
    {
        // Arrange
        var step = new PipelineStepExecution
        {
            ProjectId = 1,
            StageTypeId = 1,
            Status = "Success",
            StartedAt = DateTime.Now,
            DurationMs = 1000,
            ExitCode = 0,
            TotalErrors = 0,
            TotalWarnings = 0
        };

        // Act
        var isValid = ValidateModel(step, out var results);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(results, Is.Empty);
        });
    }

    [Test]
    public void IssueLog_WithoutRequiredFields_ShouldBeInvalid()
    {
        // Arrange
        var log = new IssueLog();

        // Act
        var isValid = ValidateModel(log, out _);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(HasValidationErrorFor(log, "Severity"), Is.True);
            Assert.That(HasValidationErrorFor(log, "Message"), Is.True);
        });
    }

    [Test]
    public void IssueLog_WithEmptySeverity_ShouldBeInvalid()
    {
        // Arrange
        var log = new IssueLog
        {
            PipelineStepExecutionId = 1,
            LoggedAt = DateTime.Now,
            Severity = string.Empty,
            Message = "Test message"
        };

        // Act
        var isValid = ValidateModel(log, out _);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(HasValidationErrorFor(log, "Severity"), Is.True);
        });
    }

    [Test]
    public void IssueLog_WithEmptyMessage_ShouldBeInvalid()
    {
        // Arrange
        var log = new IssueLog
        {
            PipelineStepExecutionId = 1,
            LoggedAt = DateTime.Now,
            Severity = "Error",
            Message = string.Empty
        };

        // Act
        var isValid = ValidateModel(log, out _);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(HasValidationErrorFor(log, "Message"), Is.True);
        });
    }

    [Test]
    public void IssueLog_WithValidData_ShouldBeValid()
    {
        // Arrange
        var log = new IssueLog
        {
            PipelineStepExecutionId = 1,
            LoggedAt = DateTime.Now,
            Severity = "Error",
            Code = "CS1001",
            Message = "Test message"
        };

        // Act
        var isValid = ValidateModel(log, out var results);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(results, Is.Empty);
        });
    }

    [Test]
    public void CpuModel_WithoutRequiredFields_ShouldBeInvalid()
    {
        // Arrange
        var cpuModel = new CpuModel();

        // Act
        var isValid = ValidateModel(cpuModel, out _);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(HasValidationErrorFor(cpuModel, "ModelName"), Is.True);
        });
    }

    [Test]
    public void CpuModel_WithEmptyModelName_ShouldBeInvalid()
    {
        // Arrange
        var cpuModel = new CpuModel
        {
            ModelName = string.Empty,
            PhysicalCoreCount = 8,
            LogicalThreadCount = 16
        };

        // Act
        var isValid = ValidateModel(cpuModel, out _);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(HasValidationErrorFor(cpuModel, "ModelName"), Is.True);
        });
    }

    [Test]
    public void CpuModel_WithValidData_ShouldBeValid()
    {
        // Arrange
        var cpuModel = new CpuModel
        {
            ModelName = "Intel Core i7",
            PhysicalCoreCount = 8,
            LogicalThreadCount = 16
        };

        // Act
        var isValid = ValidateModel(cpuModel, out var results);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(results, Is.Empty);
        });
    }

    [Test]
    public void Host_WithoutRequiredFields_ShouldBeInvalid()
    {
        // Arrange
        var host = new Host();

        // Act
        var isValid = ValidateModel(host, out _);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(HasValidationErrorFor(host, "OperatingSystem"), Is.True);
        });
    }

    [Test]
    public void Host_WithEmptyOperatingSystem_ShouldBeInvalid()
    {
        // Arrange
        var host = new Host
        {
            CpuModelId = 1,
            RamGb = 32.00m,
            OperatingSystem = string.Empty
        };

        // Act
        var isValid = ValidateModel(host, out _);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(HasValidationErrorFor(host, "OperatingSystem"), Is.True);
        });
    }

    [Test]
    public void Host_WithOperatingSystemExceedingMaxLength_ShouldBeInvalid()
    {
        // Arrange
        var host = new Host
        {
            CpuModelId = 1,
            RamGb = 32.00m,
            OperatingSystem = new string('A', 201)
        };

        // Act
        var isValid = ValidateModel(host, out _);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(HasValidationErrorFor(host, "OperatingSystem"), Is.True);
        });
    }

    [Test]
    public void Host_WithValidData_ShouldBeValid()
    {
        // Arrange
        var host = new Host
        {
            CpuModelId = 1,
            RamGb = 32.00m,
            OperatingSystem = "Windows 11"
        };

        // Act
        var isValid = ValidateModel(host, out var results);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(results, Is.Empty);
        });
    }

    [Test]
    public void StageType_WithoutName_ShouldBeInvalid()
    {
        // Arrange
        var stageType = new StageType();

        // Act
        var isValid = ValidateModel(stageType, out _);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(HasValidationErrorFor(stageType, "Name"), Is.True);
        });
    }

    [Test]
    public void StageType_WithValidData_ShouldBeValid()
    {
        // Arrange
        var stageType = new StageType
        {
            Name = "Build"
        };

        // Act
        var isValid = ValidateModel(stageType, out var results);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(results, Is.Empty);
        });
    }

    [Test]
    public void PerformanceTest_WithoutDescription_ShouldBeInvalid()
    {
        // Arrange
        var perfTest = new PerformanceTest();

        // Act
        var isValid = ValidateModel(perfTest, out _);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(HasValidationErrorFor(perfTest, "Description"), Is.True);
        });
    }

    [Test]
    public void PerformanceTest_WithValidData_ShouldBeValid()
    {
        // Arrange
        var perfTest = new PerformanceTest
        {
            Description = "Matrix Multiplication 2000x2000"
        };

        // Act
        var isValid = ValidateModel(perfTest, out var results);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(results, Is.Empty);
        });
    }

    [Test]
    public void ThreadSpeedMetric_WithValidData_ShouldBeValid()
    {
        // Arrange
        var metric = new ThreadSpeedMetric
        {
            PerformanceTestId = 1,
            HostId = 1,
            PipelineStepExecutionId = 1,
            SequentialTimeMs = 5000,
            ParallelTimeMs = 1250,
            EfficiencyCoefficient = 4.0m,
            StartedAt = DateTime.Now,
            DurationMs = 1250
        };

        // Act
        var isValid = ValidateModel(metric, out var results);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(results, Is.Empty);
        });
    }

    [Test]
    public void ThreadSpeedMetric_WithZeroEfficiency_ShouldBeValid()
    {
        // Arrange
        var metric = new ThreadSpeedMetric
        {
            PerformanceTestId = 1,
            HostId = 1,
            PipelineStepExecutionId = 1,
            SequentialTimeMs = 0,
            ParallelTimeMs = 0,
            EfficiencyCoefficient = 0,
            StartedAt = DateTime.Now,
            DurationMs = 0
        };

        // Act
        var isValid = ValidateModel(metric, out _);

        // Assert
        Assert.That(isValid, Is.True);
    }
}