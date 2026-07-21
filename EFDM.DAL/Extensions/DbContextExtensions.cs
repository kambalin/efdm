using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace EFDM.DAL.Extensions
{
    public static class DbContextExtensions
    {
        private static readonly MethodInfo SetMethodDefinition = typeof(DbContext).GetMethods()
            .Single(m => m.Name == nameof(DbContext.Set) && m.IsGenericMethodDefinition && m.GetParameters().Length == 0);
        private static readonly ConcurrentDictionary<Type, MethodInfo> SetMethodCache =
            new ConcurrentDictionary<Type, MethodInfo>();

        public static IQueryable Set(this DbContext context, Type T)
        {
            // Build a method with the specific type argument you're interested in,
            // closed methods are cached: the extension is called on audit hot paths
            var method = SetMethodCache.GetOrAdd(T, t => SetMethodDefinition.MakeGenericMethod(t));
            return method.Invoke(context, null) as IQueryable;
        }

        public static IQueryable<T> Set<T>(this DbContext context)
            => context.Set(typeof(T)) as IQueryable<T>;

        public static ModelBuilder ApplyConfiguration<T>(this ModelBuilder modelBuilder, Type configurationType, Type entityType)
        {
            if (typeof(T).IsAssignableFrom(entityType))
            {
                // Build IEntityTypeConfiguration type with generic type parameter
                var configurationGenericType = configurationType.MakeGenericType(entityType);
                // Create an instance of the IEntityTypeConfiguration implementation
                var configuration = Activator.CreateInstance(configurationGenericType);
                // Get the ApplyConfiguration method of ModelBuilder via reflection
                var applyEntityConfigurationMethod = typeof(ModelBuilder)
                    .GetMethods()
                    .Single(e => e.Name == nameof(ModelBuilder.ApplyConfiguration)
                                 && e.ContainsGenericParameters
                                 && e.GetParameters().SingleOrDefault()?.ParameterType.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>));
                // Create a generic ApplyConfiguration method with our entity type
                var target = applyEntityConfigurationMethod.MakeGenericMethod(entityType);
                // Invoke ApplyConfiguration, passing our IEntityTypeConfiguration instance
                target.Invoke(modelBuilder, new[] { configuration });
            }
            return modelBuilder;
        }
    }
}
