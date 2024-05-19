namespace Api.Repositories;

public static class RepositoriesExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped<IEmployeeRepository, InMemoryRepository>()
            .AddScoped<IDependentRepository, InMemoryRepository>();
    }
}