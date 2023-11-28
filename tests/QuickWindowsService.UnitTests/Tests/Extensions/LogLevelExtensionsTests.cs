using Microsoft.Extensions.Logging;
using QuickWindowsService.Extensions;
using Serilog.Events;
using System;

namespace QuickWindowsService.UnitTests.Tests.Extensions;

public class LogLevelExtensionsTests
{
    [Fact]
    public void Test_OutOfRange()
    {
        const LogLevel input = (LogLevel) 9999;
        Assert.Throws<ArgumentOutOfRangeException>(() => input.ToSerilogLogEventLevel());
    }

    [Theory]
    [InlineData(LogLevel.None, LogEventLevel.Fatal)] // best-effort
    [InlineData(LogLevel.Trace, LogEventLevel.Verbose)]
    [InlineData(LogLevel.Debug, LogEventLevel.Debug)]
    [InlineData(LogLevel.Error, LogEventLevel.Error)]
    [InlineData(LogLevel.Warning, LogEventLevel.Warning)]
    [InlineData(LogLevel.Critical, LogEventLevel.Fatal)]
    [InlineData(LogLevel.Information, LogEventLevel.Information)]
    public void Test_ToSerilogLevel(LogLevel input, LogEventLevel? expectedOutput)
    {
        Assert.Equal(expectedOutput, input.ToSerilogLogEventLevel());
    }
}