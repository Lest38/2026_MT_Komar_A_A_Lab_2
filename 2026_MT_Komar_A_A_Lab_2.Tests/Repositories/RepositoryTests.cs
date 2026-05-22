using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using _2026_MT_Komar_A_A_Lab_2.Tests.Helpers;

namespace _2026_MT_Komar_A_A_Lab_2.Tests.Repositories;

[TestFixture]
public class RepositoryTests
{
    private TestDatabaseFixture fixture = null!;

    [SetUp] public void SetUp() => fixture = new TestDatabaseFixture();
    [TearDown] public void TearDown() => fixture.Dispose();

    [Test]
    public async Task AddAsync_ValidEntity_IsPersisted()
    {
        await using var context = fixture.CreateContext();
        var repo = new Repository<Project>(context);
        var project = new Project { Name = "Add Test", FolderPath = @"C:\Repo\Add" };

        await repo.AddAsync(project);
        await context.SaveChangesAsync();

        var saved = await context.Projects.FirstOrDefaultAsync(p => p.Name == "Add Test");
        Assert.That(saved, Is.Not.Null);
        Assert.That(saved!.FolderPath, Is.EqualTo(@"C:\Repo\Add"));
    }

    [Test]
    public void AddAsync_NullEntity_ThrowsArgumentNullException()
    {
        using var context = fixture.CreateContext();
        var repo = new Repository<Project>(context);
        Assert.ThrowsAsync<ArgumentNullException>(() => repo.AddAsync(null!));
    }

    [Test]
    public async Task GetByIdAsync_ExistingId_ReturnsEntity()
    {
        await using var context = fixture.CreateContext();
        var repo = new Repository<Project>(context);
        var project = new Project { Name = "GetById", FolderPath = @"C:\Repo\GetById" };

        await repo.AddAsync(project);
        await context.SaveChangesAsync();

        var result = await repo.GetByIdAsync(project.Id);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.Id, Is.EqualTo(project.Id));
            Assert.That(result.Name, Is.EqualTo("GetById"));
        });
    }

    [Test]
    public async Task GetByIdAsync_NonExistentId_ReturnsNull()
    {
        await using var context = fixture.CreateContext();
        var repo = new Repository<Project>(context);

        var result = await repo.GetByIdAsync(99999);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAllAsync_MultipleEntities_ReturnsAll()
    {
        await using var context = fixture.CreateContext();
        var repo = new Repository<Project>(context);

        var projects = new[]
        {
            new Project { Name = "All-1", FolderPath = @"C:\All\1" },
            new Project { Name = "All-2", FolderPath = @"C:\All\2" },
            new Project { Name = "All-3", FolderPath = @"C:\All\3" },
        };

        foreach (var p in projects)
            await repo.AddAsync(p);
        await context.SaveChangesAsync();

        var result = await repo.GetAllAsync();
        Assert.That(result.Count(), Is.EqualTo(3));
    }

    [Test]
    public async Task DeleteAsync_ExistingEntity_IsRemoved()
    {
        await using var context = fixture.CreateContext();
        var repo = new Repository<Project>(context);
        var project = new Project { Name = "Delete Me", FolderPath = @"C:\Delete\Me" };

        await repo.AddAsync(project);
        await context.SaveChangesAsync();

        await repo.DeleteAsync(project);
        await context.SaveChangesAsync();

        var deleted = await context.Projects.FindAsync(project.Id);
        Assert.That(deleted, Is.Null);
    }

    [Test]
    public async Task ExistsAsync_NoMatch_ReturnsFalse()
    {
        await using var context = fixture.CreateContext();
        var repo = new Repository<Project>(context);

        var exists = await repo.ExistsAsync(p => p.Name == "DoesNotExist_XYZ");
        Assert.That(exists, Is.False);
    }

    [Test]
    public void FindAsync_NullPredicate_ThrowsArgumentNullException()
    {
        using var context = fixture.CreateContext();
        var repo = new Repository<Project>(context);
        Assert.ThrowsAsync<ArgumentNullException>(() => repo.FindAsync(null!));
    }
}