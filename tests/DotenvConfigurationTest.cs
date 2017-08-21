using System;
using System.IO;
using System.Collections.Generic;
using Xunit;

using Microsoft.Extensions.Configuration;

using Frozzare.Dotenv;

using TestHelper;


namespace Frozzare.Dotenv.Tests
{
    public class DotenvConfigurationTest : IDisposable
    {
		private TemporaryTestFile _TestFile;
		private ConfigurationBuilder _Builder;
		protected IConfigurationRoot _Config;
		private string _ConfigBody;

		public DotenvConfigurationTest() {
			_ConfigBody = @"
				Foobar = ""hi there""
				Logging_LogLevel_Simple = ""Special""
				Logging:IncludeScopes = false
				Logging:LogLevel_Default = ""Warning""
				Logging:LogLevel_Microsoft = ""Error""
				Logging:LogLevel:Flux = ""Trace""
				Logging:LogLevel:System = ""Blue""
			";
			_TestFile = new TemporaryTestFile(_ConfigBody);
			SetupConfig(_TestFile);
		}

		public void Dispose() {
			_TestFile.Dispose();
		}

		private void SetupConfig(TemporaryTestFile temp) {
			_Builder = new ConfigurationBuilder();
			_Builder.AddDotenvFile( temp.Path );
			_Config = _Builder.Build();
		}

		[Fact]
		public void simple_key_can_be_fetched() {
			Assert.Equal("hi there", _Config["Foobar"]);
		}

		[Fact]
		public void long_key_can_be_fetched() {
			Assert.Equal("Special", _Config["Logging_LogLevel_Simple"]);
		}

		[Fact]
		public void key_in_section_can_be_retrieved() {
			const string SECTION_KEY = "LogLevel_Default";
			var section = _Config.GetSection("Logging");
			Assert.NotNull( section[ SECTION_KEY ]);
			Assert.True( section[SECTION_KEY].Length > 0 );
		}

		public class when_in_subsection_section : DotenvConfigurationTest {
			IConfigurationSection _Logging;
			IConfigurationSection _LogLevel;

			public when_in_subsection_section() : base() {
				_Logging = _Config?.GetSection("Logging") ?? null;
				_LogLevel = _Logging?.GetSection("LogLevel") ?? null;
			}

			[Fact]
			public void subsection_of_subsection_can_be_dereferenced() {
				Assert.Equal("Trace", _LogLevel["Flux"]);
			}

		}
    }
}
