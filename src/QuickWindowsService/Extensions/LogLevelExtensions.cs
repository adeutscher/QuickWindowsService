using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace QuickWindowsService.Extensions;

internal static class LogLevelExtensions
{
    public static LogEventLevel ToSerilogLogEventLevel(this LogLevel microsoftLevel)
    {
        switch (microsoftLevel)
        {
            case LogLevel.Trace:
                return LogEventLevel.Verbose;
            case LogLevel.Debug:
                return LogEventLevel.Debug;
            case LogLevel.Information:
                return LogEventLevel.Information;
            case LogLevel.Warning:
                return LogEventLevel.Warning;
            case LogLevel.Error:
                return LogEventLevel.Error;
            case LogLevel.Critical:
                return LogEventLevel.Fatal;
            // Microsoft intends None to mean no logs, but Serilog has no equivalent
            // Best-worst option is to use Serilog's highest level of filtering
            case LogLevel.None:
                return LogEventLevel.Fatal;
            default:
                throw new ArgumentOutOfRangeException(nameof(microsoftLevel), microsoftLevel, null);
        }
    }
}