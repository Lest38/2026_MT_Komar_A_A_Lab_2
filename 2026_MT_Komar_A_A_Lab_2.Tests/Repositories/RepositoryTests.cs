using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using _2026_MT_Komar_A_A_Lab_2.Tests.Helpers;

namespace _2026_MT_Komar_A_A_Lab_2.Tests.Repositories;

[TestFixture]
public class RepositoryTests
{
    private TestDatabaseFixture _fixture;

    [SetUp]
    public void SetUp()
    {
        _fixture = new TestDatabaseFixture();
    }

    [TearDown]
    public void TearDown()
    {
        _fixture?.Dispose();
    }

    [Test]
    public async Task AddAsync_ShouldAddEntityToDatabase()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new Repository<Project>(context);
        var project = new Project
        {
            Name = "Test Project",
            FolderPath = @"C:\Test\Project"
        };

        // Act
        await repository.AddAsync(project);
        await context.SaveChangesAsync();

        // Assert
        var savedProject = await context.Projects.FirstOrDefaultAsync(p => p.Name == "Test Project");
        Assert.That(savedProject, Is.Not.Null);
        Assert.That(savedProject.FolderPath, Is.EqualTo(project.FolderPath));
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnCorrectEntity()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new Repository<Project>(context);
        var project = new Project
        {
            Name = "GetById Test",
            FolderPath = @"C:\Test\GetById"
        };
        await repository.AddAsync(project);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(project.Id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(project.Id));
            Assert.That(result.Name, Is.EqualTo(project.Name));
        });
    }

    [Test]
    public async Task GetByIdAsync_WhenEntityDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new Repository<Project>(context);

        // Act
        var result = await repository.GetByIdAsync(99999);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new Repository<Project>(context);

        var projects = new[]
        {
            new Project { Name = "Project 1", FolderPath = @"C:\Test\1" },
            new Project { Name = "Project 2", FolderPath = @"C:\Test\2" },
            new Project { Name = "Project 3", FolderPath = @"C:\Test\3" }
        };

        foreach (var project in projects)
        {
            await repository.AddAsync(project);
        }
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(3));
    }

    [Test]
    public async Task UpdateAsync_ShouldUpdateEntity()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new Repository<Project>(context);
        var project = new Project
        {
            Name = "Original Name",
            FolderPath = @"C:\Test\Original"
        };
        await repository.AddAsync(project);
        await context.SaveChangesAsync();

        // Act
        project.Name = "Updated Name";
        await repository.UpdateAsync(project);
        await context.SaveChangesAsync();

        // Assert
        var updatedProject = await context.Projects.FindAsync(project.Id);
        Assert.That(updatedProject?.Name, Is.EqualTo("Updated Name"));
    }

    [Test]
    public async Task DeleteAsync_ShouldRemoveEntity()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new Repository<Project>(context);
        var project = new Project
        {
            Name = "Delete Me",
            FolderPath = @"C:\Test\Delete"
        };
        await repository.AddAsync(project);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(project);
        await context.SaveChangesAsync();

        // Assert
        var deletedProject = await context.Projects.FindAsync(project.Id);
        Assert.That(deletedProject, Is.Null);
    }

    [Test]
    public async Task ExistsAsync_ShouldReturnTrue_WhenEntityExists()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new Repository<Project>(context);
        var project = new Project
        {
            Name = "Exists Test",
            FolderPath = @"C:\Test\Exists"
        };
        await repository.AddAsync(project);
        await context.SaveChangesAsync();

        // Act
        var exists = await repository.ExistsAsync(p => p.Name == "Exists Test");

        // Assert
        Assert.That(exists, Is.True);
    }

    [Test]
    public async Task FindAsync_ShouldReturnMatchingEntities()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new Repository<Project>(context);

        var projects = new[]
        {
            new Project { Name = "Find Me 1", FolderPath = @"C:\Test\Find1" },
            new Project { Name = "Find Me 2", FolderPath = @"C:\Test\Find2" },
            new Project { Name = "Other", FolderPath = @"C:\Test\Other" }
        };

        foreach (var project in projects)
        {
            await repository.AddAsync(project);
        }
        await context.SaveChangesAsync();

        // Act
        var result = await repository.FindAsync(p => p.Name.StartsWith("Find Me"));

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
    }
}