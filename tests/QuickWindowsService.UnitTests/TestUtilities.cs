using System;
using System.Collections.Generic;

namespace QuickWindowsService.UnitTests;

public class TestUtilities
{
    /// <summary>
    ///     Following AWS' standard for unit tests with environment variables.
    ///     Making this wrapper because I didn't want to individually save/restore values in multiple tests.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="callback"></param>
    public static void WrapEnvironment(Dictionary<string, string> values, Action callback)
    {
        var savedRecords = new Dictionary<string, string>();

        // save
        foreach (var keyValuePair in values)
        {
            savedRecords[keyValuePair.Key] = Environment.GetEnvironmentVariable(keyValuePair.Key);
        }

        try
        {
            // set
            foreach (var keyValuePair in values)
            {
                Environment.SetEnvironmentVariable(keyValuePair.Key, keyValuePair.Value);
            }

            callback();
        }
        finally
        {
            foreach (var keyValuePair in savedRecords)
            {
                Environment.SetEnvironmentVariable(keyValuePair.Key, keyValuePair.Value);
            }
        }
    }
}