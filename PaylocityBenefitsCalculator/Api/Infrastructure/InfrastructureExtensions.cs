using Api.Core.Repositories;
using Api.Infrastructure.Data;
using Api.Infrastructure.Repositories;
using Api.Infrastructure.Settings;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureLayerServices(this IServiceCollection services, IConfigurationManager configurationManager)
    {
        var dataContextSettings = configurationManager.GetSection(DataContextSettings.SECTION_NAME).Get<DataContextSettings>()!;
        services.AddSingleton(dataContextSettings);

        // note: using sql lite just for example - in real world scenario, we would use a real database (e.g. SQL Server, Postgres, etc.)
        // switch with only one value in enum is here just for demonstration purpose - this is a way how we can switch between in-memory and real database using the appsettings / env values ...
        // with this approach we can have for example sqlite database for local development + automatic tests and real database for production
        switch (dataContextSettings.DatabaseKind)
        {
            case DatabaseKind.InMemory:
                services.AddDbContext<DataContext>((Action<DbContextOptionsBuilder>) (o =>
                {
                    var sqliteConnection = new SqliteConnection(dataContextSettings.ConnectionString);
                    sqliteConnection.Open();
                    o.UseSqlite(sqliteConnection);
                }));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return services
            .AddScoped<DatabaseInitializer>()
            .AddScoped<IEmployeeRepository, EmployeeRepository>()
            .AddScoped<IDependentRepository, DependentRepository>();
    }

    public static async Task InitializeInfrastructure(this IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            // note: here we can check if database is created / all migrations applied etc... I will just initialize DB with original data here.
            // when we will use real database we can move the test data initialization to the test project 
            var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
            var settings = scope.ServiceProvider.GetRequiredService<DataContextSettings>();
            if (settings.DatabaseKind == DatabaseKind.InMemory)
            {
                await dataContext.Database.EnsureCreatedAsync();
                await databaseInitializer.Initialize();
            }
        }

    }
}