using EFDM.Sample.Core.Constants.System;
using EFDM.Sample.DAL.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace EFDM.Sample.Npgs.Providers;

public class DbContextFactory : IDesignTimeDbContextFactory<TestDatabaseContext>
{
    public TestDatabaseContext CreateDbContext(string[] args)
    {
        var configuration = GetConfiguration();
        var connectionString = configuration
            .GetSection(SettingsValuesNames.ConnectionStringPg)?
            .Get<string>();

        var builder = new DbContextOptionsBuilder<TestDatabaseContext>()
            .UseNpgsql(connectionString, options =>
            {
                options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });
        return new TestDatabaseContext(builder.Options);
    }

    protected IConfiguration GetConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.MachineName}.json", true, true)
            .Build();
    }
}

