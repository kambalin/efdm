using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Core.Extensions {

    public static class DbContextExtensions {

        public static IQueryable Set(this DbContext context, Type T) {
            // Get the generic type definition
            MethodInfo method = new Func<DbSet<object>>(context.Set<object>).Method.GetGenericMethodDefinition();            

            // Build a method with the specific type argument you're interested in
            method = method.MakeGenericMethod(T);

            return method.Invoke(context, null) as IQueryable;
        }

        public static IQueryable<T> Set<T>(this DbContext context) {
            // Get the generic type definition 
            MethodInfo method = new Func<DbSet<object>>(context.Set<object>).Method.GetGenericMethodDefinition();

            // Build a method with the specific type argument you're interested in 
            method = method.MakeGenericMethod(typeof(T));

            return method.Invoke(context, null) as IQueryable<T>;
        }

        public static ModelBuilder ApplyConfiguration<T>(this ModelBuilder modelBuilder, Type configurationType, Type entityType) {
            if (typeof(T).IsAssignableFrom(entityType)) {
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
