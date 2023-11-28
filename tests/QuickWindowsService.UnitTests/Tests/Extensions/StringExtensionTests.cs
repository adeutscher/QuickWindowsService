using QuickWindowsService.Extensions;
using Serilog.Events;

namespace QuickWindowsService.UnitTests.Tests.Extensions;

public class StringExtensionTests
{
    [Theory]
    [InlineData("Foo", null)]
    [InlineData("Verbose", LogEventLevel.Verbose)]
    [InlineData("Trace", LogEventLevel.Verbose)]
    [InlineData("Fatal", LogEventLevel.Fatal)]
    [InlineData("Critical", LogEventLevel.Fatal)]
    [InlineData("Information", LogEventLevel.Information)]
    public void Test_ToSerilogLevel(string input, LogEventLevel? expectedOutput)
    {
        Assert.Equal(expectedOutput, input.ToSerilogLevel());
    }
}