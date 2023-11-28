using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace QuickWindowsService.Services;

internal class MainService : BackgroundService
{
    private readonly ConfigurationModel _configuration;
    private readonly IDelayWrapper _delayWrapper;
    private readonly ILogger<MainService> _logger;
    private readonly IProgramExecutor _programExecutor;

    public MainService(IProgramExecutor programExecutor, IDelayWrapper delayWrapper,
        IOptions<ConfigurationModel> configuration, ILogger<MainService> logger)
    {
        _delayWrapper = delayWrapper;
        _logger = logger;
        _programExecutor = programExecutor;
        _configuration = configuration.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (string.IsNullOrWhiteSpace(_configuration.ProgramPath))
        {
            _logger.LogError("No program path set.");
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            await _programExecutor.RunAsync(_configuration.ProgramPath, _configuration.ProgramArguments);
            await _delayWrapper.DelaySecondsAsync(_configuration.RestDelaySeconds, stoppingToken);
        }
    }

    public class ConfigurationModel
    {
        public string ProgramPath { get; init; }
        public string ProgramArguments { get; init; }
        public int RestDelaySeconds { get; init; }
    }
}