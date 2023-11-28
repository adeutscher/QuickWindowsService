using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace QuickWindowsService.Services;

public interface IProgramExecutor
{
    Task<int> RunAsync(string executablePath, string arguments);
}

// ReSharper disable once SuggestBaseTypeForParameterInConstructor
internal class ProgramExecutor(ILogger<ProgramExecutor> logger) : IProgramExecutor
{
    public async Task<int> RunAsync(string executablePath, string arguments)
    {
        logger.LogDebug("Executing program: {ExecutablePath} {Arguments}", executablePath, arguments);
        var process = new Process();
        var startInfo = new ProcessStartInfo
        {
            WindowStyle = ProcessWindowStyle.Hidden,
            FileName = executablePath,
            Arguments = arguments
        };
        process.StartInfo = startInfo;
        process.Start();
        await process.WaitForExitAsync();
        logger.LogDebug("Executed program (code {ProcessExitCode}): {ExecutablePath} {Arguments}", process.ExitCode,
            executablePath, arguments);
        return process.ExitCode;
    }
}