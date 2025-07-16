using Microsoft.Extensions.Configuration;

namespace Journey.Infrastructure.Extensions;

public static class AppSettingsConfigHelper
{
    public static IConfiguration Configuration { get; private set; }
    public static void SetConfiguration(IConfiguration config)
    {
        Configuration = config;
    }
}