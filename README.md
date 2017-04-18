# Dotenv

> Work in progress

Use `.env` with ASP.NET Core.

This package will try to go around [FileSystem #232](https://github.com/aspnet/FileSystem/issues/232) bug that don't allow hidden files.

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
}
```

You can use a `.env` file with any of the following features:

```shell
# comments are allowed
FOO=bar # you can also have comments on the end of a line
export BAR=baz # you can optionally begin with an `export` statement

# both single and double quotes are allowed
BAZ='qux'
QUX="quux"

# as are escaped quotes or similar:
QUUX="corge \" grault"
CORGE='garply" waldo'

# when quoted, they are simply string values
STRING_NULL="null"
STRING_TRUE="true"
STRING_FALSE="false"

# spaces are allowed as well
# in a slightly more relaxed form from bash
 GRAULT =fred
GARPLY = plugh
SPACES=" quote values with spaces" # will contain preceding space

# When using newlines, you should use quoted values
QUOTED_NEWLINE="newline\\nchar"

# you can even have nested variables using `${VAR}` syntax
# remember to define the nested var *before* using it
WALDO=${xyzzy} # not yet defined, so will result in WALDO = `{}`
THUD=${GARPLY} # will be defined as `plugh`

# note that variables beginning with a character
# other than [a-zA-Z_] shall throw a ParseException
01SKIPPED=skipped

# However, numbers *are* allowed elsewhere in the key
NOT_SKIPPED1=not skipped # will have the value `not`
```

# License

MIT © [Fredrik Forsmo](https://github.com/frozzare)