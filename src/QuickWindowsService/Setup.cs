using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuickWindowsService.Extensions;

namespace QuickWindowsService;

public static class Setup
{
    public static void ConfigureServices(IServiceCollection serviceCollection, string environmentOverride = null)
    {
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariablesWithSections()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables()
            .Build();

        serviceCollection
            .AddOptions()
            .AddLogging()
            .ConfigureServices(configuration)
            .ConfigureSerilog("quick_windows_service");
    }
}