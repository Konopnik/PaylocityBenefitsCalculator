using Api.Core.Services;
using Api.Core.Settings;
using Microsoft.Extensions.Options;

namespace Api.Core;

public static class ApplicationExtensions
{
    public static IServiceCollection AddCoreLayerServices(this IServiceCollection services)
    {
        services.AddOptions<PaycheckCalculatorSettings>(PaycheckCalculatorSettings.SECTION_NAME);
        services.AddSingleton<PaycheckCalculatorSettings>(resolver => resolver.GetService<IOptions<PaycheckCalculatorSettings>>()!.Value);

        return services.AddScoped<IPaycheckCalculator, PaycheckCalculator>();
    }
}