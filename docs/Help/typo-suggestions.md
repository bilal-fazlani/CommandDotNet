# Typo Suggestions

## TLDR, How to enable 
Enable the feature with `appRunner.UseTypoSuggestions()` or `appRunner.UseDefaultMiddleware()`.

## Introduction
When a user types in a command or option that does not exist, this feature will suggest similar commands or options in case the user made a typo.

``` bash
λ fakegit commish
'commish' is not a valid subcommand

Did you mean ...
   commit

See 'fakegit --help'
~
λ fakegit commit --brash
'brash' is not a valid option

Did you mean ...
   --branch

See 'fakegit commit --help'
```

Operand names will never be suggested as they are never entered by the user.

When the typo is prefixed with `--` then similar options will be suggested, else similar commands are suggested.

Option short names are not included in typo suggestions.

## Suggestions for argument values

When `IArgument.AllowedValues` is populated, TypoSuggestions will offer suggestions based on the allowed values as well.

`IArgument.AllowedValues` are populated by [TypeDescriptors](../Arguments/argument-types.md#type-descriptors) implementing [IAllowedValuesTypeDescriptor](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/TypeDescriptors/IAllowedValuesTypeDescriptor.cs) which currently includes `bool` and `enum` types.

``` bash
λ schedule-task --day Mondday
'Mondday' is not a valid DayOfWeek

Did you mean ...
   Monday

See 'schedule-task --help'
```

To provide AllowedValues for other types, you can 

* create a [TypeDescriptors](../Arguments/argument-types.md#type-descriptors) for your type
* subscribe to `OnCommandCreated` to update arguments as they're added
    * `appRunner.Configure(c =>  c.BuildEvents.OnCommandCreated += args => a.CommandBuilder.Command.Options ... /*set AllowedValues*/)` 
* add middleware to set the `IArgument.AllowedValues` after `MiddlewareSteps.ParseInput`
    * `appRunner.Configure(c =>  c.UseMiddleware((ctx,next)=> ctx.ParseResult.TargetCommand.Options ... /*set AllowedValues*/, MiddlewareSteps.ParseInput + 100))` 
