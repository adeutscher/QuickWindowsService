using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using QuickWindowsService.Extensions;
using System.Collections.Generic;

namespace QuickWindowsService.UnitTests.Tests.Extensions;

public class ConfigurationBuilderExtensionTests
{
    /// <summary>
    ///     Happy-path integration test of configuration.
    /// </summary>
    [Fact]
    public void TestBasic()
    {
        const string base1 = "http://12345";
        const string base2 = "http://another";
        const string base3 = "http://another-again";

        var environment = new Dictionary<string, string>
        {
            {"A__B__BASEURL", base1},
            {"A__B__C__AnotherBaseUrl", base2},
            {"A__B__C__Yet_Another_Base_Url", base3}
        };

        TestUtilities.WrapEnvironment(environment, () =>
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariablesWithSections()
                .Build();

            Assert.Equal(base1, configuration["A:B:BaseUrl"]);
            Assert.Equal(base2, configuration["A:B:C:AnotherBaseUrl"]);
            Assert.Equal(base3, configuration["A:B:C:YetAnotherBaseUrl"]);
            Assert.Equal(base3, configuration["A:B:C:Yet_Another_Base_Url"]);
        });
    }

    /// <summary>
    ///     Happy-path test of converting the IDictionary to a Dictionary
    /// </summary>
    [Fact]
    public void Test_GetEnvironmentDictionary()
    {
        var input = new Dictionary<string, string>
        {
            {"foo", "bar"}
        };

        var output = ConfigurationBuilderExtensions.GetEnvironmentDictionary(input);

        foreach (var inputEntry in input)
        {
            var entry = Assert.Single(output, outputEntry => outputEntry.Key == inputEntry.Key);
            Assert.Equal(inputEntry.Value, entry.Value);
        }
    }

    [Fact]
    public void Test_GetEnvironmentDictionary_EmptyKey()
    {
        var input = new Dictionary<string, string>
        {
            {string.Empty, "bar"}
        };

        var output = ConfigurationBuilderExtensions.GetEnvironmentDictionary(input);

        Assert.Empty(output);
    }

    [Fact]
    public void Test_GetEnvironmentDictionary_EmptyValue()
    {
        var input = new Dictionary<string, string>
        {
            {"foo", string.Empty}
        };

        var output = ConfigurationBuilderExtensions.GetEnvironmentDictionary(input);

        Assert.Empty(output);
    }

    [Fact]
    public void Test_GetEnvironmentDictionary_NullValue()
    {
        var input = new Dictionary<string, string>
        {
            {"foo", null}
        };

        var output = ConfigurationBuilderExtensions.GetEnvironmentDictionary(input);

        Assert.Empty(output);
    }

    [Fact]
    public void Test_GetFlattenedEnvironmentVariables_Basic()
    {
        var input = new Dictionary<string, string>
        {
            {"foo_bar", "baz"},
            {"redherring", "ignored"}
        };

        var output = ConfigurationBuilderExtensions.GetFlattenedEnvironmentVariables(input);

        var entry = Assert.Single(output);
        Assert.Equal("foobar", entry.Key);
        Assert.Equal(input["foo_bar"], entry.Value);
    }

    /// <summary>
    ///     Confirm translation of double-underscores into Microsoft's section phrasing of ':'
    /// </summary>
    [Fact]
    public void Test_TranslateEnvironmentDictionarySections_Basic()
    {
        const string value = "1234";
        var input = new Dictionary<string, string>
        {
            {"foo__bar", value}
        };
        var output = ConfigurationBuilderExtensions.TranslateEnvironmentDictionarySections(input);

        var entry = Assert.Single(output);
        Assert.Equal("foo:bar", entry.Key);
        Assert.Equal(value, entry.Value);
    }

    internal class TestHandler
    {
        public TestConfigurationModel Config { get; }
        public TestSubConfigurationModel SubConfig { get; }

        public TestHandler(IOptions<TestConfigurationModel> config, IOptions<TestSubConfigurationModel> subConfig)
        {
            Config = config.Value;
            SubConfig = subConfig.Value;
        }

        public class TestConfigurationModel
        {
            public string BaseUrl { get; set; }
        }

        public class TestSubConfigurationModel
        {
            public string AnotherBaseUrl { get; set; }
            public string YetAnotherBaseUrl { get; set; }
        }
    }
}