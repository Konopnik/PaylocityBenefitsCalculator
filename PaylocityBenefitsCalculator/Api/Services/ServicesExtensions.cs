using Api.Repositories;

namespace Api.Services;

public static class ServicesExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IPaycheckCalculator, PaycheckCalculator>();
    }
}