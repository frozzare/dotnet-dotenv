
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
        private string _ConfigBody;
        private ConfigurationBuilder _Builder;

        public DotenvConfigurationContentTest()
        {
            _ConfigBody = @"
				Foobar = ""hi there""
				Love_underscore = ""True""
				Logging_LogLevel_Simple = ""Special""
				Logging:IncludeScopes = false
				Logging:LogLevel_Default = ""Warning""
				Logging:LogLevel_Microsoft = ""Error""
				Logging:LogLevel:Flux = ""Trace""
				Logging:LogLevel:System = ""Blue""
			";
            _Builder = new ConfigurationBuilder();
        }

        public class _files_from_filesystem_ : DotenvConfigurationContentTest, IDisposable
        {
            private TemporaryTestFile _TestFile;
            protected IConfigurationRoot _Config;

            public _files_from_filesystem_()
            {
                _TestFile = new TemporaryTestFile(_ConfigBody);
                SetupConfig(_TestFile);
            }

            public void Dispose()
            {
                _TestFile.Dispose();
            }

            private void SetupConfig(TemporaryTestFile temp)
            {
                _Builder.AddDotenvFile(path: temp.Path);
                _Config = _Builder.Build();
            }

            [Fact]
            public void simple_key_can_be_fetched()
            {
                Assert.Equal("hi there", _Config["Foobar"]);
            }

        }

        public class _mock_file_provider_ : DotenvConfigurationContentTest
        {
            private MockFileProvider _MockProvider;
            protected IConfigurationRoot _Config;

            public _mock_file_provider_() : base()
            {
                _MockProvider = new MockFileProvider(_ConfigBody);
                _Builder.AddDotenvFile(provider: _MockProvider, path: _MockProvider.Path, optional: false, reloadOnChange: false);
                _Config = _Builder.Build();
            }

            [Fact]
            public void long_key_can_be_fetched()
            {
                Assert.Equal("Special", _Config["Logging_LogLevel_Simple"]);
            }

            [Fact]
            public void key_in_section_can_be_retrieved()
            {
                const string SECTION_KEY = "LogLevel_Default";
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
                    _Logging = _Config?.GetSection("Logging") ?? null;
                    _LogLevel = _Logging?.GetSection("LogLevel") ?? null;
                }

                [Fact]
                public void subsection_of_subsection_can_be_dereferenced()
                {
                    Assert.Equal("Trace", _LogLevel["Flux"]);
                }
            }

        }

    }

}
