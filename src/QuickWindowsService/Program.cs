using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuickWindowsService;
using QuickWindowsService.Services;

var host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureServices(services =>
    {
        Setup.ConfigureServices(services);
        services.AddHostedService<MainService>();
    })
    .Build();

host.Run();