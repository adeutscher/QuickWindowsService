using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("QuickWindowsService.UnitTests")]

namespace QuickWindowsService.Extensions;

public static class ConfigurationBuilderExtensions
{
    private static IDictionary Combine(params IDictionary[] inputs)
    {
        var output = new Dictionary<string, string>();
        foreach (var subitem in inputs)
        {
            foreach (DictionaryEntry kvp in subitem)
            {
                output[kvp.Key.ToString()!] = kvp.Value?.ToString()!;
            }
        }

        return output;
    }

    /// <summary>
    ///     Load in environment variables in a manner mimicking JSON sections using '__' as delimiters.
    ///     FOO__BAR is equivalent to a JSON configuration with the phrasing '{ "FOO": { "BAR": "value" }}'
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    /// <summary>
    ///     Parse environment variables into sections (divided by '__') for granularity in the resulting IConfiguration object.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IConfigurationBuilder AddEnvironmentVariablesWithSections(this IConfigurationBuilder builder)
    {
        var environmentVariablesUser = GetEnvironmentDictionary(Environment.GetEnvironmentVariables());
        var environmentVariablesMachine =
            GetEnvironmentDictionary(Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine));
        var environmentVariablesCombined = Combine(environmentVariablesMachine, environmentVariablesUser);
        var environmentDictionary = GetEnvironmentDictionary(environmentVariablesCombined);
        var environmentVariablesSectioned = TranslateEnvironmentDictionarySections(environmentDictionary);
        var environmentVariablesFlattened = GetFlattenedEnvironmentVariables(environmentVariablesSectioned);
        return builder.AddInMemoryCollection(environmentVariablesSectioned.Concat(environmentVariablesFlattened));
    }

    /// <summary>
    ///     Translate IDictionary to a Dictionary
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    internal static Dictionary<string, string> GetEnvironmentDictionary(IDictionary input)
    {
        var output = new Dictionary<string, string>();

        foreach (DictionaryEntry de in input)
        {
            var key = de.Key?.ToString();
            if (string.IsNullOrWhiteSpace(key))
            {
                continue;
            }

            var value = de.Value?.ToString();
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            output[key] = value;
        }

        return output;
    }

    internal static Dictionary<string, string> GetFlattenedEnvironmentVariables(Dictionary<string, string> input)
    {
        var output = new Dictionary<string, string>();

        foreach (var keyValuePair in input)
        {
            var moddedKey = keyValuePair.Key.Replace("_", string.Empty);
            if (moddedKey == keyValuePair.Key)
            {
                continue;
            }

            output[moddedKey] = keyValuePair.Value;
        }

        return output;
    }

    internal static Dictionary<string, string> TranslateEnvironmentDictionarySections(
        Dictionary<string, string> input)
    {
        var output = new Dictionary<string, string>();

        foreach (var kvp in input)
        {
            output[kvp.Key.Replace("__", ":")] = kvp.Value;
        }

        return output;
    }
}