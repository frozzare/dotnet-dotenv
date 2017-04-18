# Dotenv

> Work in progress

## Installation

...

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