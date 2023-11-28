using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace QuickWindowsService.Extensions;

internal static class StringExtensions
{
    internal static LogEventLevel? ToSerilogLevel(this string? input)
    {
        if (Enum.TryParse<LogEventLevel>(input, out var output))
        {
            return output;
        }

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (Enum.TryParse<LogLevel>(input, out var microsoftLevel))
        {
            return microsoftLevel.ToSerilogLogEventLevel();
        }

        return null;
    }
}