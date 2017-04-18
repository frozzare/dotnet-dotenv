using Microsoft.Extensions.Configuration;

namespace Frozzare.Dotenv
{
    /// <summary>
    /// Represents dotenv variables as an <see cref="IConfigurationSource"/>.
    /// </summary>
    public class DotenvConfigurationSource : FileConfigurationSource
    {
        /// <summary>
        /// Builds the <see cref="DotenvConfigurationProvider"/> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>A <see cref="DotenvConfigurationProvider"/></returns>
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            FileProvider = FileProvider ?? builder.GetFileProvider();
            return new DotenvConfigurationProvider(this);
        }
    }
}