using Entities;

namespace _2026_MT_Komar_A_A_Lab_2.Tests.Entities;

[TestFixture]
public class BaseEntityStructureTests : EntityStructureTestBase<BaseEntity<int>>
{
    [Test]
    public void BaseEntity_IsAbstract_NotConcrete()
    {
        Assert.That(EntityType.IsAbstract, Is.True,
            "BaseEntity<int> must be abstract – it is not a concrete entity");
    }

    [Test]
    public void BaseEntity_DoesNotInheritBaseEntity()
    {
        EntityDoesNotInheritBaseEntity();
    }
}