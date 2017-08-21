
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

    public class DotenvConfigurationContentTest
    {
        const string _ConfigBody= @"
			Foobar = ""hi there""
			Love_underscore = ""True""
			Logging_LogLevel_Simple = ""Special""
			Logging:IncludeScopes = false
			Logging:LogLevel_Default = ""Warning""
			Logging:LogLevel_Microsoft = ""Error""
			Logging:LogLevel:Flux = ""Trace""
			Logging:LogLevel:System = ""Blue""
		";

        public DotenvConfigurationContentTest()
        {
        }

        public class _files_from_filesystem_ : DotenvConfigurationContentTest, IDisposable
        {
            private TemporaryTestFile _TestFile;
            protected IConfigurationRoot _Config;
            private ConfigurationBuilder _Builder;

            public _files_from_filesystem_()
            {
            }

            public void Dispose()
            {
                _TestFile.Dispose();
                _TestFile = null;
                _Builder = null;
                _Config = null;
            }

            private void SetupConfig()
            {
                _TestFile = new TemporaryTestFile(_ConfigBody);
                _Builder = new ConfigurationBuilder();
                _Builder.AddDotenvFile(path: _TestFile.Path);
                _Config = _Builder.Build();
            }

            [Fact]
            public void simple_key_can_be_fetched()
            {
                SetupConfig();
                Assert.Equal("hi there", _Config["Foobar"]);
            }

        }

        public class _mock_file_provider_ : DotenvConfigurationContentTest
        {
            private MockFileProvider _MockProvider;
            protected IConfigurationRoot _Config;
            private ConfigurationBuilder _Builder;

            public _mock_file_provider_() : base()
            {
            }

            protected void Setup() {
                _Builder = new ConfigurationBuilder();
                _MockProvider = new MockFileProvider(_ConfigBody);
                _Builder.AddDotenvFile(provider: _MockProvider, path: _MockProvider.Path, optional: false, reloadOnChange: false);
                _Config = _Builder.Build();
            }

            [Fact]
            public void long_key_can_be_fetched()
            {
                Setup();
                Assert.Equal("Special", _Config["Logging_LogLevel_Simple"]);
            }

            [Fact]
            public void key_in_section_can_be_retrieved()
            {
                const string SECTION_KEY = "LogLevel_Default";
                Setup();
                var section = _Config.GetSection("Logging");

                Assert.NotNull(section[SECTION_KEY]);
                Assert.True(section[SECTION_KEY].Length > 0);
            }

            public class when_in_subsection_section : _mock_file_provider_
            {
                IConfigurationSection _Logging;
                IConfigurationSection _LogLevel;

                public when_in_subsection_section() : base()
                {
                    Setup();
                    _Logging = _Config?.GetSection("Logging") ?? null;
                    _LogLevel = _Logging?.GetSection("LogLevel") ?? null;
                }

                [Fact]
                public void subsection_of_subsection_can_be_dereferenced()
                {
                    Setup();
                    Assert.Equal("Trace", _LogLevel["Flux"]);
                }
            }

        }

    }

}
