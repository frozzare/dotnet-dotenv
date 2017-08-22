/*
* Written by Warwick Molloy GitHub|@WazzaMo
* to ensure AspNetCore log configuration can be
* specified as per AspNetCore documentation
* and code templates.
*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using Xunit;
using Microsoft.Extensions.Configuration;
using Frozzare.Dotenv;
using TestHelper;


namespace Frozzare.Dotenv.Tests
{
    public class TestConfig : IDisposable
    {
        //-- Dotenv can have race conditions when loaded by concurrent threads.
        private static Mutex _Mutex = new Mutex(false, "EnvironmentVariables Lock");

        public const string DEFAULT_CONTENT = @"
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

        public TestConfig(string body = null)
        {
            string content = body != null ? body : DEFAULT_CONTENT;
            SetupConfig(content);
        }

        public void Dispose()
        {
            TestFile.Dispose();
            TestFile = null;
            Builder = null;
            Config = null;
            _Mutex.ReleaseMutex();
        }

        protected void SetupConfig(string content)
        {
            try
            {
                _Mutex.WaitOne();
                TestFile = new TemporaryTestFile(content);
                Builder = new ConfigurationBuilder();
                Builder.AddDotenvFile(TestFile.Path);
                Config = Builder.Build();
            }
            catch(Exception e)
            {
                System.Console.WriteLine(
                    $"Exception on build:\n"
                    +$"  Problem: {e.Message}\n"
                    +$"  Stack:   {e.StackTrace}\n"
                );
            }
            finally
            {
            }
        }
    }
}