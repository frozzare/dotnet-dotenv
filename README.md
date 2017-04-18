# Dotenv

> Work in progress

Use `.env` with ASP.NET Core.

This package will try to go around [FileSystem #232](https://github.com/aspnet/FileSystem/issues/232) that don't allow hidden files.

## Installation

TBA

## Usage

```csharp
public class Startup
{
  public Startup(IHostingEnvironment env)
  {
    var builder = new ConfigurationBuilder()
      .SetBasePath(env.ContentRootPath)
      .AddDotenvFile(".env", optional: false, reloadOnChange: true);
    Configuration = builder.Build();
  }
  
  public IConfigurationRoot Configuration { get; }
}
```

# License

MIT © [Fredrik Forsmo](https://github.com/frozzare)