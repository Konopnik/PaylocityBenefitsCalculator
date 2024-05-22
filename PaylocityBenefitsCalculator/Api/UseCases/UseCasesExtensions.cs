using Api.UseCases.Employees;

namespace Api.UseCases;

public static class UseCasesExtensions
{
    public static IServiceCollection AddUseCasesLayerService(this IServiceCollection services)
    {
        return services.AddSingleton<UseCasesMapper>();
    }
}