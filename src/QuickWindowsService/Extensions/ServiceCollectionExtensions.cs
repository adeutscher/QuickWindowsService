using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuickWindowsService.Services;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace QuickWindowsService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureSerilog(this IServiceCollection services, string prefix)
    {
        const string logPathEnvVar = "QUICK_WINDOWS_SERVICE__LOG_PATH";
        const string logLvlEnvVar = "QUICK_WINDOWS_SERVICE__LOG_MINIMUM_LEVEL";

        // Determine log path
        var logPath = Environment.GetEnvironmentVariable(logPathEnvVar) ?? // User-scope
                      Environment.GetEnvironmentVariable(logPathEnvVar,
                          EnvironmentVariableTarget.Machine) ?? // Machine-scope
                      Path.Combine(Directory.GetCurrentDirectory(),
                          "logs"); // Fallback -- current dir that the program was invoked from
        if (!Directory.Exists(logPath))
        {
            Directory.CreateDirectory(logPath);
        }

        // Determine log level

        var logLvlStr = Environment.GetEnvironmentVariable(logLvlEnvVar) ?? // User-scope
                        Environment.GetEnvironmentVariable(logLvlEnvVar,
                            EnvironmentVariableTarget.Machine); // Machine-scope
        var logLvl = logLvlStr.ToSerilogLevel() ?? LogEventLevel.Information;

        // Sets log path to be some envVar, if set, or execDir /logs, then **/{prefix}_<lvl>-<day>.txt
        // Sets log level to be Verbose in VS debug, envVar or Information in file and console
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .WriteTo.File(restrictedToMinimumLevel: logLvl,
                path: Path.Combine(logPath, $"{prefix}_{logLvl.ToString().ToLower()}-.txt"),
                rollingInterval: RollingInterval.Day,
                formatter: new RenderedCompactJsonFormatter())
            .WriteTo.Console(logLvl,
                "{Timestamp:o} [{Level:u3}]{Message}{NewLine}{Exception}")
            .WriteTo.Debug(
                outputTemplate: "{Timestamp:o} [{Level:u3}] {Message}{NewLine}{Exception}")
            .CreateLogger();

        return services
            .AddLogging(builder =>
            {
                builder
                    .AddSerilog(dispose: true);
            });
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services,
        IConfigurationRoot configuration)
    {
        services = services
            .AddOptions()
            .AddSingleton<IDelayWrapper, DelayWrapper>()
            .AddSingleton<IProgramExecutor, ProgramExecutor>()
            .AddSingleton<MainService>()
            .Configure<MainService.ConfigurationModel>(configuration.GetSection("QuickWindowsService"))
            .ConfigureSerilog("quick-windows-service");

        return services;
    }
}