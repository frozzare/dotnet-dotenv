using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace Frozzare.Dotenv
{
    public static class DotenvExtension
    {
        /// <summary>
        /// Adds the Dotenv configuration provider at default path to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="path">Path relative to the base path stored in
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddDotenvFile(this IConfigurationBuilder builder)
        {
            return AddDotenvFile(builder, null, string.Empty, false, false);
        }

        /// <summary>
        /// Adds the Dotenv configuration provider at <paramref name="path"/> to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="path">Path relative to the base path stored in
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddDotenvFile(this IConfigurationBuilder builder, string path)
        {
            return AddDotenvFile(builder, null, path, false, false);
        }

        /// <summary>
        /// Adds the Dotenv configuration provider at <paramref name="path"/> to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="path">Path relative to the base path stored in
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <param name="optional">Whether the file is optional.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddDotenvFile(this IConfigurationBuilder builder, string path, bool optional)
        {
            return AddDotenvFile(builder, null, path, optional, false);
        }

        /// <summary>
        /// Adds the Dotenv configuration provider at <paramref name="path"/> to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="path">Path relative to the base path stored in
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <param name="optional">Whether the file is optional.</param>
        /// <param name="reloadOnChange">Whether the configuration should be reloaded if the file changes.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddDotenvFile(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange)
        {
            return AddDotenvFile(builder, null, path, optional, reloadOnChange);
        }

        /// <summary>
        /// Adds a Dotenv configuration source to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="provider">The <see cref="IFileProvider"/> to use to access the file.</param>
        /// <param name="path">Path relative to the base path stored in
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <param name="optional">Whether the file is optional.</param>
        /// <param name="reloadOnChange">Whether the configuration should be reloaded if the file changes.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddDotenvFile(this IConfigurationBuilder builder, IFileProvider provider, string path, bool optional, bool reloadOnChange)
        {
            // Bail if builder is null.
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            // Use the default value if value is empty.
            if (string.IsNullOrEmpty(path))
            {
                path = Dotenv.DefaultPath;
                path = path.Replace("./", "");
            }

#if NET451
            var basePath1 = AppDomain.CurrentDomain.GetData("APP_CONTEXT_BASE_DIRECTORY") as string
                ?? AppDomain.CurrentDomain.BaseDirectory
                ?? string.Empty;
#else
            var basePath1 = AppContext.BaseDirectory ?? string.Empty;
#endif
            var basePaths = new List<string>() { basePath1 };

            // Since we shouldn't relay on asp.net core hosting package
            // and then we can't get content root path and because of
            // https://github.com/aspnet/FileSystem/issues/232 
            // we have to create two different paths that we can try to read the dotenv file from.
            if (basePath1.Contains("/bin/"))
            {
                var basePath2 = basePath1.Split(new string[] { "bin" }, StringSplitOptions.None)[0];
                basePaths.Add(basePath2.TrimEnd('/'));
            }

            var fileExists = false;
            foreach (var basePath in basePaths)
            {
                var testPath = string.Join("/", new string[] { basePath, path });
                if (File.Exists(testPath))
                {
                    fileExists = true;
                    path = testPath;
                    break;
                }
            }

            if (fileExists)
            {
                if (provider == null && Path.IsPathRooted(path))
                {
                    // Real PhysicalFileProvider has a bug that don't allow dot files:
                    // https://github.com/aspnet/FileSystem/issues/232
                    provider = new FileProvider.PhysicalFileProvider(Path.GetDirectoryName(path));
                    path = Path.GetFileName(path);
                }

                var source = new DotenvConfigurationSource
                {
                    Path = path,
                    Optional = optional,
                    FileProvider = provider,
                    ReloadOnChange = reloadOnChange
                };
                builder.Add(source);
			}
            return builder;
        }
    }
}
