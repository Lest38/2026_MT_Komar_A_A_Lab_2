using _2026_MT_Komar_A_A_Lab_2.Tests.Helpers;

namespace _2026_MT_Komar_A_A_Lab_2.Tests.UnitOfWork;

[TestFixture]
public class UnitOfWorkTests
{
    private TestDatabaseFixture fixture = null!;

    [SetUp] public void SetUp() => fixture = new TestDatabaseFixture();
    [TearDown] public void TearDown() => fixture.Dispose();

    [Test]
    public void AllRepositories_AreNotNull_AfterConstruction()
    {
        using var context = fixture.CreateContext();
        using var unitOfWork = new global::UnitsOfWork.UnitOfWork(context);

        Assert.Multiple(() =>
        {
            Assert.That(unitOfWork.Projects, Is.Not.Null);
            Assert.That(unitOfWork.StageTypes, Is.Not.Null);
            Assert.That(unitOfWork.PipelineStepExecutions, Is.Not.Null);
            Assert.That(unitOfWork.IssueLogs, Is.Not.Null);
            Assert.That(unitOfWork.CpuModels, Is.Not.Null);
            Assert.That(unitOfWork.Hosts, Is.Not.Null);
            Assert.That(unitOfWork.PerformanceTests, Is.Not.Null);
            Assert.That(unitOfWork.ThreadSpeedMetrics, Is.Not.Null);
        });
    }

    [Test]
    public void Dispose_CalledOnce_DoesNotThrow()
    {
        using var context = fixture.CreateContext();
        var unitOfWork = new global::UnitsOfWork.UnitOfWork(context);

        Assert.DoesNotThrow(() => unitOfWork.Dispose());
    }
}