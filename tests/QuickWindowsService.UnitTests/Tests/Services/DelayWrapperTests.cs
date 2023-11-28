using QuickWindowsService.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace QuickWindowsService.UnitTests.Tests.Services;

public class DelayWrapperTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task Test_DelayAsync(int amount)
    {
        var delayWrapper = new DelayWrapper();

        var stopwatch = Stopwatch.StartNew();
        await delayWrapper.DelaySecondsAsync(amount);
        stopwatch.Stop();

        var thresholdMinimum = amount * 1000 - 50;
        Assert.True(stopwatch.Elapsed >= TimeSpan.FromMilliseconds(thresholdMinimum));

        var thresholdMaximum = amount * 1000 + 100;
        Assert.True(stopwatch.Elapsed <= TimeSpan.FromMilliseconds(thresholdMaximum));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task Test_DelayAsync_NoTime(int amount)
    {
        var delayWrapper = new DelayWrapper();

        var stopwatch = Stopwatch.StartNew();

        await delayWrapper.DelaySecondsAsync(amount);

        stopwatch.Stop();

        Assert.True(stopwatch.Elapsed < TimeSpan.FromMilliseconds(10));
    }
}