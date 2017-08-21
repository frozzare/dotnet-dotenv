using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Frozzare.Dotenv
{
    public class Dotenv
    {
        /// <summary>
        /// Default dotenv file path.
        /// </summary>
        public static string DefaultPath = "./.env";

        /// <summary>
        /// Parsed environment variables.
        /// </summary>
        protected Dictionary<string, string> variables = new Dictionary<string, string>();

        /// <summary>
        /// Load dotenv file, parse file and add variables as environment variables.
        /// </summary>
        /// <param name="path">File path to dotenv file</param>
        /// <returns>Dotenv instance.</returns>
        public static Dotenv Load(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                path = DefaultPath;
            }

            if (!File.Exists(path))
            {
                throw new Exception("The .env file don't exists in the current directory");
            }

            var content = File.ReadAllText(path);
            
            return new Dotenv(content);
        }

        /// <summary>
        /// Load dotenv from stream, parse and add variables as environment variables.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <returns>Dotenv instance.</returns>
        public static Dotenv Load(Stream input)
        {
            return new Dotenv(new StreamReader(input).ReadToEnd());
        }

        /// <summary>
        /// Get environment variables.
        /// </summary>
        /// <returns>Dictionary of environment variables.</returns>
        public Dictionary<string, string> GetVariables()
        {
            return variables;
        }

        /// <summary>
        /// Load .env file, parse file and add variables as environment variables.
        /// </summary>
        /// <param name="content">File content.</param>
        protected Dotenv(string content)
        {
            var parsedVars = parseContent(content);

            foreach (var variable in parsedVars)
            {
                var key = variable.Key;
                var value = variable.Value;

                foreach (var var in parseValue(value))
                {
                    // When variable is not defined the result should be "{}".
                    var replace = String.IsNullOrEmpty(parsedVars[var]) ? "{}" : parsedVars[var];
                    value = value.Replace("${" + var + "}", replace);
                }

                Environment.SetEnvironmentVariable(key, value);
                variables[key] = value;
            }
        }

        /// <summary>
        /// Parse value and extract parameters.
        /// </summary>
        /// <param name="value">Environment variable value.</param>
        /// <returns>List of parameters.</returns>
        protected IList<string> parseValue(string value)
        {
            var vars = new List<string>();
            var regex = new Regex(@"\$\{(.*?)\}");

            foreach (Match match in regex.Matches(value))
            {
                if (!match.Success)
                {
                    continue;
                }

                vars.Add(match.Groups[1].Value);
            }

            return vars;
        }

        /// <summary>
        /// Parse file content.
        /// </summary>
        /// <param name="lines">Array of lines to parse.</param>
        /// <returns>Dictionary of key values.</returns>
        protected Dictionary<string, string> parseContent(string content)
        {
            var lines = content.Split('\n');
            var vars = new Dictionary<string, string>();
            var regex = new Regex(@"^(?:export|)\s*([^\d+][:\w_]+)\s?=\s?(.+)");
            for (int i = 0; i < lines.Length; i++)
            {
                var matches = regex.Match(lines[i]);
                var key = matches.Groups[1].Value;
                var value = String.Empty;

                // Bail if empty key.
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }

                // Replace empty value with real value if any.
                if (!string.IsNullOrEmpty(matches.Groups[2].Value))
                {
                    value = matches.Groups[2].Value;
                }

                // Split string that don't starts with a quote.
                if (!value.StartsWith("\"") && !value.StartsWith("\'"))
                {
                    value = value.Split(' ')[0];
                }

                // Remove quotes in the beging and the end of a string.
                value = Regex.Replace(value, "^(?:\"|\')|(?:\"|\')$", string.Empty);

                vars.Add(key, value);
            }

            return vars;
        }
    }
}
