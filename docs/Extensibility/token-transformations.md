# Token Transformations

Token transformations are a way to tranform input arguments before they are mapped to commands.

This feature is used internally to expand clubbed options and support [Response Files](../ArgumentValues/response-files.md).

See these implementations as examples:

  * [ExpandClubbedOptionsTransformation](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/Tokens/ExpandClubbedOptionsTransformation.cs)
  * [ExpandResponseFilesTransformation](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/Tokens/ExpandResponseFilesTransformation.cs)

## How to implement

First, create a function with the signature `Func<CommandContext, TokenCollection, TokenCollection>`.
This function will transform a `TokenTransformation` into a new `TokenTransformation`.

Next, register the transformation with 

```c#
appRunner.Configure(c => c.UseTokenTransformation(
    name: "my-transformation", 
    order: 1, 
    transformation: MyFunc));
```

* __name__ is used for logging and the [Parse Directive](directives.md)
* __order__ is the order the transformation should run in relation to other transformations.

## Tokens

Tokens are initially parsed from the Program.Main args array. Transformations can modify tokens or expand them into additional tokens.

Tokens contains

* __RawValue__: Raw value from the user input. This will contain the punctuation used to denote option and argument names or brackets enclosing directives. (eg. `--help`, `[debug]`)
* __Value__: Can be an Option name or an argument value or a directive(eg. `help`) 
* __TokenType__: Directive, Option, Value, Separator
* __OptionTokenType__: when TokenType is Option

OptionTokenType contains

* __IsLong__: is a long name (`--help`)
* __IsShort__: is a short name (`-h`)
* __IsClubbed__: multiple short names in a single string (`-hv`).
* __HasValue__: the option has a value assignment (`--verbosity:trace`)

## TokenCollections

TokenCollections are an immutable collection of tokens with some convenience members to simplify transformations.

* __Directives__: The tokens interpreted as [directives](directives.md). This will always be empty when `AppSettings.DisableDirectives = false`.
* __Arguments__: All arguments after the last directive and before the argument separator `--`
* __Separated__: All arguments included after the argument separator  `--`. If there are multiple separators, the remaining separators will be tokens in this collection.


!!! Tip
    Use TokenCollection.Transform method to apply a func to tokens of specific types. 
    This is an alternative to a foreach loop with a switch statement.
    See the implementations linked above for examples.

    Use TokenCollection.ToArgsArray extension method to convert the tokens into a string
    array that can be passed to another application.