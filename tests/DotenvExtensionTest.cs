
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
    public class DotenvExtensionTest
    {
        protected ConfigurationBuilder _Builder;
        protected IConfigurationRoot _Config;
        private string _ConfigBody;

        public DotenvExtensionTest()
        {
            _ConfigBody = @"
                Foobar = ""hi there""
                Logging:IncludeScopes = false
                Logging:LogLevel_Default = ""Warning""
                Logging:LogLevel_Microsoft = ""Error""
                Logging:LogLevel:Flux = ""Trace""
                Logging:LogLevel:Simple = ""Special""
                Logging:LogLevel:System = ""Blue""
            ";
        }

        public class _AddDotenvFile_all_arguments_overload : DotenvExtensionTest
        {
            protected MockFileProvider _MockProvider;
            protected int PriorUseCount;

            public _AddDotenvFile_all_arguments_overload() : base()
            {
                _MockProvider = new MockFileProvider(_ConfigBody);
                _Builder = new ConfigurationBuilder();
            }

            IConfigurationBuilder Subject()
            {
                return _Builder.AddDotenvFile(_MockProvider, _MockProvider.Path, false, false);
            }

            [Fact]
            public void uses_FileProvider_to_check_existence()
            {
                _MockProvider.PretendFileExists(true);
                _MockProvider.SetUsageAction(() => throw new ArgumentException("Success provider was used!"));
                Assert.Throws<ArgumentException>(() => Subject());
            }

            [Fact]
            public void valid_builder_and_path_arguments()
            {
                _MockProvider.PretendFileExists(true);
                Assert.NotNull(Subject());
            }
        }

    }
}
