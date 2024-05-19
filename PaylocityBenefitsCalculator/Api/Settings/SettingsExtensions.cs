using Api.Services;
using Microsoft.Extensions.Options;

namespace Api.Settings;

public static class SettingsExtensions
{
    public static IServiceCollection AddSettings(this IServiceCollection services)
    {
        services.AddOptions<PaycheckCalculatorSettings>(PaycheckCalculatorSettings.SECTION_NAME);
        services.AddSingleton<PaycheckCalculatorSettings>(resolver => resolver.GetService<IOptions<PaycheckCalculatorSettings>>()!.Value);
        return services;
    }
}