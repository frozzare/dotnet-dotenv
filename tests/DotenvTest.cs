using System;
using System.IO;
using System.Collections.Generic;
using Xunit;
using Frozzare.Dotenv;

namespace Frozzare.Dotenv.Tests
{
    public class DotenvTest
    {
        protected Stream createStream(string str)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        [Fact]
        public void TestVariables()
        {
            var tests = new Dictionary<KeyValuePair<string, string>, string>(){
                {new KeyValuePair<string, string>("FOO", "FOO=bar"), "bar"},
                {new KeyValuePair<string, string>("BAR", "export BAR=foo"), "foo"},
                {new KeyValuePair<string, string>("BAZ", "BAZ='qux'"), "qux"},
                {new KeyValuePair<string, string>("QUX", "QUX=\"quux\""), "quux"},
                {new KeyValuePair<string, string>("QUUX", "QUUX=\"corge \" grault\""), "corge \" grault"},
                {new KeyValuePair<string, string>("CORGE", "CORGE='garply\" waldo'"), "garply\" waldo"},
                {new KeyValuePair<string, string>("GARPLY","GARPLY = plugh"), "plugh"},
                {new KeyValuePair<string, string>("QUOTED_NEWLINE","QUOTED_NEWLINE=\"newline\\nchar\""), "newline\\nchar"},
                {new KeyValuePair<string, string>("FOO", "BAR=foo\nFOO=${BAR}"), "foo"},
                {new KeyValuePair<string, string>("FOO", "HELLO=world\nBAR=foo\nFOO=${HELLO}+${BAR}"), "world+foo"},
                {new KeyValuePair<string, string>("NOT_SKIPPED1", "NOT_SKIPPED1=not skipped"), "not"}
            };
            
            foreach (var test in tests)
            {
                var path = Directory.GetCurrentDirectory() + "/.env";
                File.WriteAllText(path, test.Key.Value);

                var dotenv = Dotenv.Load(path);
                Assert.Equal(test.Value, Environment.GetEnvironmentVariable(test.Key.Key.Trim()));
            }
        }

        [Fact]
        public void TestStreamVariables()
        {
            var stream = createStream("BAR=foo\nFOO=${BAR}");
            var dotenv = Dotenv.Load(stream);

            Assert.Equal("foo", Environment.GetEnvironmentVariable("FOO"));
        }

        [Fact]
        public void TestProvider()
        {
            var p = new DotenvConfigurationProvider(new DotenvConfigurationSource {});
            p.Load(createStream("BAR=foo\nFOO=${BAR}"));

            string value;
            p.TryGet("FOO", out value);

            Assert.Equal("foo", value);
        }

        [Fact]
        public void TestSkippedVariables()
        {
            
            var tests = new Dictionary<KeyValuePair<string, string>, string>(){
                {new KeyValuePair<string, string>("01SKIPPED", "01SKIPPED=skipped"), null}
            };

            foreach (var test in tests)
            {
                var path = Directory.GetCurrentDirectory() + "/.env";
                File.WriteAllText(path, test.Key.Value);

                var dotenv = Dotenv.Load(path);
                Assert.Equal(test.Value, Environment.GetEnvironmentVariable(test.Key.Key.Trim()));
            }
        }
    }
}
