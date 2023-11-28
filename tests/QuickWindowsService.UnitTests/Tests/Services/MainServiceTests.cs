using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using QuickWindowsService.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QuickWindowsService.UnitTests.Tests.Services;

public class MainServiceTests
{
    [Theory]
    [InlineData(2, 4)]
    [InlineData(4, 2)]
    public async Task Test_ExecuteAsync(int delaySeconds, int iterations)
    {
        var cts = new CancellationTokenSource(1234);

        var configuration = new MainService.ConfigurationModel
        {
            ProgramPath = Guid.NewGuid().ToString(),
            ProgramArguments = Guid.NewGuid().ToString(),
            RestDelaySeconds = delaySeconds
        };
        var executor = new Mock<IProgramExecutor>();
        var delayer = new Mock<IDelayWrapper>();
        var iterationsExecuted = 0;
        delayer.Setup(d => d.DelaySecondsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Callback((int _, CancellationToken _) =>
            {
                iterationsExecuted++;
                if (iterationsExecuted == iterations)
                {
                    cts.Cancel();
                }
            });
        var service = new MockService(executor.Object, delayer.Object, Options.Create(configuration),
            new NullLogger<MainService>());
        await service.RunAsync(cts.Token);

        var timesExpected = Times.Exactly(iterations);

        executor.Verify(e => e.RunAsync(It.IsAny<string>(), It.IsAny<string>()), timesExpected);
        executor.Verify(e => e.RunAsync(configuration.ProgramPath, configuration.ProgramArguments), timesExpected);

        delayer.Verify(d => d.DelaySecondsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), timesExpected);
        delayer.Verify(d => d.DelaySecondsAsync(delaySeconds, It.IsAny<CancellationToken>()), timesExpected);
        delayer.Verify(d => d.DelaySecondsAsync(It.IsAny<int>(), cts.Token), timesExpected);
    }

    /// <summary>
    ///     Making a mock class around MainService in order to get at the protected ExecuteAsync method.
    /// </summary>
    internal class MockService(IProgramExecutor programExecutor, IDelayWrapper delayWrapper,
            IOptions<MainService.ConfigurationModel> configuration, ILogger<MainService> logger)
        : MainService(programExecutor, delayWrapper, configuration, logger)
    {
        public Task RunAsync(CancellationToken cancellationToken)
        {
            return base.ExecuteAsync(cancellationToken);
        }
    }
}