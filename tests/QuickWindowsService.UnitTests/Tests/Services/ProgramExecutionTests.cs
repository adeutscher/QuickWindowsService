using Microsoft.Extensions.Logging.Abstractions;
using QuickWindowsService.Services;

namespace QuickWindowsService.UnitTests.Tests.Services;

public class ProgramExecutionTests
{
    [Fact]
    public async void Test_RunAsync()
    {
        var executor = new ProgramExecutor(new NullLogger<ProgramExecutor>());

        var code = await executor.RunAsync("echo", "a");
        Assert.Equal(0, code);
    }
}