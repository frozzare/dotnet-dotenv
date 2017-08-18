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
		private IConfigurationRoot _Config;
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
    }
}
