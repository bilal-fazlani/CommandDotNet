# Directives

Directives are special arguments enabling cross cutting features.  We've loosely followed the pattern defined by  [System.CommandLine](https://github.com/dotnet/command-line-api/wiki/Features-overview#debugging) to start with two directives, [Debug](../Diagnostics/debug-directive.md) & [Parse](../Diagnostics/parse-directive.md).

Directives are a great way to add troubleshooting tools to your application. See [Custom Directives](#custom-directives) at the bottom of this page for tips on adding your own.

## How to use
Directives must be the first arguments, enclosed by square brackets and will be removed from the arguments list during tokenization. After the first non-directive token is processed, any following tokens with square brackets will be processed as commands and operand values.

```bash
dotnet example.dll [some-directive] -v [not-a-directive]
```

To disable the directives feature entirely, set `AppSettings.DisableDirectives = true`. This is only needed if an application has starting arguments enclosed by square brackets and using the [argument separator](../ArgumentValues/argument-separator.md) is not a suitable solution.

## Custom directives

Directives are middleware components.  
See the implementations of [Debug](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/Directives/DebugDirective.cs) 
and [Parse](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/Directives/ParseDirective.cs) directives
for examples.

The presence of a directive is checked with `commandContext.Tokens.TryGetDirective("debug", out _))`.  The out parameter will include the entire string within the square brackets. Using a possible logging directive `[log:debug]`, the out parameter value would be `log:debug`. You can parse the value however you'd like.