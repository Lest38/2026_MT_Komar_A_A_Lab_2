using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _2026_MT_Komar_A_A_Lab_2.Tests.Entities
{
    [TestFixture]
    public abstract class EntityStructureTestBase<TEntity>
    where TEntity : class
    {
        protected static Type EntityType => typeof(TEntity);

        protected static void EntityInheritsBaseEntity()
        {
            Assert.That(
                EntityType.BaseType,
                Is.EqualTo(typeof(BaseEntity<int>)));
        }

        protected static void EntityDoesNotInheritBaseEntity()
        {
            Assert.That(
                EntityType.BaseType,
                Is.Not.EqualTo(typeof(BaseEntity<int>)));
        }

        protected static void IsPublicEntity()
        {
            Assert.Multiple(() =>
            {
                Assert.That(EntityType.IsClass, Is.True, $"{EntityType.Name} must be a class");
                Assert.That(EntityType.IsPublic, Is.True, $"{EntityType.Name} must be public");
                Assert.That(EntityType.IsAbstract, Is.False, $"{EntityType.Name} must not be abstract");
            });
        }

        protected static PropertyInfo HasPublicProperty(string propertyName, Type expectedPropertyType)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public;
            var propertyInfo = EntityType.GetProperty(propertyName, flags);

            Assert.That(propertyInfo, Is.Not.Null, $"{EntityType.Name} must have property '{propertyName}'");
            Assert.Multiple(() =>
            {
                Assert.That(propertyInfo!.PropertyType, Is.EqualTo(expectedPropertyType), $"'{propertyName}' must be of type {expectedPropertyType.Name}");
                Assert.That(propertyInfo.GetMethod!.IsPublic, Is.True, $"'{propertyName}' getter must be public");
                Assert.That(propertyInfo.SetMethod!.IsPublic, Is.True, $"'{propertyName}' setter must be public");
            });

            return propertyInfo!;
        }

        protected static PropertyInfo HasPublicReadOnlyProperty(string propertyName, Type expectedPropertyType)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public;
            var propertyInfo = EntityType.GetProperty(propertyName, flags);

            Assert.That(propertyInfo, Is.Not.Null, $"{EntityType.Name} must have property '{propertyName}'");
            Assert.Multiple(() =>
            {
                Assert.That(propertyInfo!.PropertyType, Is.EqualTo(expectedPropertyType), $"'{propertyName}' must be of type {expectedPropertyType.Name}");
                Assert.That(propertyInfo.GetMethod!.IsPublic, Is.True, $"'{propertyName}' getter must be public");
            });

            return propertyInfo!;
        }

        protected static void ImplementsIEntity()
        {
            Assert.That(
                typeof(IEntity<int>).IsAssignableFrom(EntityType),
                Is.True,
                $"{EntityType.Name} must implement IEntity<int>");
        }
    }
}
