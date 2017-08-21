
/*
 * Written by Warwick Molloy GitHub|@WazzaMo
 * to ensure AspNetCore log configuration can be
 * specified as per AspNetCore documentation
 * and code templates.
 */

using System;
using System.IO;
using System.Collections.Generic;
using Xunit;
using Microsoft.Extensions.Configuration;
using Frozzare.Dotenv;
using TestHelper;

namespace Frozzare.Dotenv.Tests
{
    public class TestConfig : IDisposable
    {
        public const string _ConfigBody = @"
    		Foobar = ""hi there""
    		Logging_LogLevel_Simple = ""Special""
    		Logging:IncludeScopes = false
    		Logging:LogLevel_Default = ""Warning""
    		Logging:LogLevel_Microsoft = ""Error""
    		Logging:LogLevel:Flux = ""Trace""
    		Logging:LogLevel:System = ""Blue""
    	";
        public TemporaryTestFile TestFile;
        public ConfigurationBuilder Builder;
        public IConfigurationRoot Config;

        public TestConfig()
        {
            SetupConfig();
        }

        public void Dispose()
        {
            TestFile.Dispose();
        }

        protected void SetupConfig()
        {
            TestFile = new TemporaryTestFile(_ConfigBody);
            Builder = new ConfigurationBuilder();
            Builder.AddDotenvFile(TestFile.Path);
            Config = Builder.Build();
        }
    }

    public class DotenvConfigurationTest 
    {

        [Fact]
        public void simple_key_can_be_fetched()
        {
            using( var Test = new TestConfig() )
            {
                Assert.Equal("hi there", Test.Config["Foobar"]);
            }
        }

        [Fact]
        public void long_key_can_be_fetched()
        {
            using( var Test = new TestConfig() )
            {
                Assert.Equal("Special", Test.Config["Logging_LogLevel_Simple"]);
            }
        }

        [Fact]
        public void key_in_section_can_be_retrieved()
        {
            const string SECTION_KEY = "LogLevel_Default";
            using( var Test = new TestConfig() )
            {
                var section = Test.Config.GetSection("Logging");
                Assert.NotNull(section[SECTION_KEY]);
                Assert.True(section[SECTION_KEY].Length > 0);
            }
        }

        public class when_in_subsection_section : DotenvConfigurationTest
        {
            IConfigurationSection _Logging;
            IConfigurationSection _LogLevel;

            public when_in_subsection_section() : base()
            {
            }

            [Fact]
            public void subsection_of_subsection_can_be_dereferenced()
            {
                using( var Test = new TestConfig() )
                {
                    _Logging = Test.Config?.GetSection("Logging") ?? null;
                    _LogLevel = _Logging?.GetSection("LogLevel") ?? null;
                    Assert.Equal("Trace", _LogLevel["Flux"]);
                }
            }
        }
    }
}
