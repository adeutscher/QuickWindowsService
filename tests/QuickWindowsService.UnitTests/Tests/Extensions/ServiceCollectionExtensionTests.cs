using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuickWindowsService.Extensions;
using QuickWindowsService.Services;
using System.Collections.Generic;

namespace QuickWindowsService.UnitTests.Tests.Extensions;

public class ServiceCollectionExtensionTests
{
    /// <summary>
    ///     Test that Function.ConfigureServices provides everything that we need.
    /// </summary>
    [Fact]
    public void Test_DependencyInjection_ConfigureServices()
    {
        var configuration = new ConfigurationBuilder().Build();
        var serviceCollection = new ServiceCollection();

        serviceCollection.ConfigureServices(configuration);

        serviceCollection
            // Trust service setup to handle MainService, options, and logging.
            .AddSingleton<MainService>()
            .AddOptions()
            .AddLogging();

        var environment = new Dictionary<string, string>();

        TestUtilities.WrapEnvironment(environment, () =>
        {
            var handler = serviceCollection
                .BuildServiceProvider()
                .GetRequiredService<MainService>();
            Assert.NotNull(handler);
        });
    }
}