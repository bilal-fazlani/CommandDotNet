# Arguments

Arguments are defined as parameters of a method or properties of a class (see [Argument Models](argument-models.md))

As discussed in [Terminology](../argument-terminology.md), Option and Operand are the two concrete types of IArgument

* Option: named argument
* Operand: positional argument

By default, arguments are operands. Change the default by assigning `#!c# AppSettings.DefaultArgumentMode = ArgumentMode.Option`

Use the `[Operand]` and `[Option]` attributes to explicity denote which argument type and to configure the arguments.

!!!Tip
    See [Option vs Operand](option-or-operand.md) for recommendations on when to use one vs the other.

## Operand Attribute

The operand attribute has the following properties:

* __Name__: Used in help documentation only. Defaults to the parameter or property name.
* __Description__: Used in help documentation.

## Option Attribute

The option attribute has the following properties:

* __LongName__: Used in help documentation and on the command line. Defaults to the parameter or property name. 
    * Set to `null` to remove the default long name and have only a short name.
* __ShortName__: Used in help documentation and on the command line. Defaults to null.
* __Description__: Used in help documentation.
* __BooleanMode__: When the option is a `bool`, this determines if the presence of the option 
  indicates `true` (_Implicit_) or if the user must specify `true` or `false` (_Explicit_). 
    * The default is _Implicit_ and can be changed with `#!c# AppSettings.BooleanMode = BooleanMode.Explicit`
    * _Implicit_ boolean options are also called __Flags__
* __AssignToExecutableSubcommands__: only valid when used in [Interceptor](../Extensibility/interceptors.md) methods.

## Example

``` c#
public void LaunchRocket(
    [Operand(Name = "planet", 
        Description = "Name of the planet you wish the rocket to go")] 
    string planetName,
    [Option(LongName = "turbo", ShortName = "t", 
        Description = "Name of the planet you wish the rocket to go")] 
    bool turbo,
    [Option(ShortName="a",
         Description="Abort the launch before takeoff", 
         BooleanMode=BooleanMode.Explicit)]
    bool abort)
```

and help looks like:

```bash
Usage: dotnet CommandDotNet.Example.dll launch-rocket [options] [arguments]  
                                                                             
Arguments:                                                                   
                                                                             
  planet  <TEXT>                                                             
  Name of the planet you wish the rocket to go                               
                                                                             
Options:                                                                     
                                                                             
  -t | --turbo                                                               
  Name of the planet you wish the rocket to go                               
                                                                             
  -a      <BOOLEAN>                                                    
  Abort the launch before takeoff                                            
  Allowed values: true, false                                                
```

and called any of these ways:

```bash
dotnet example.dll LaunchRocket -t -a true mars
dotnet example.dll LaunchRocket --turbo -a true mars
dotnet example.dll LaunchRocket mars -t -a true
dotnet example.dll LaunchRocket mars --turbo -a true
```

Options are not positional so they can appear in any order within the command.

Configuring a ShortName nullifies the default long name 
so LongName must also be configured if you need both.

| Parameter name | Longname | Shortname | Generated template
| --- | --- | --- | --- |
| turbo |  |  | `--turbo` |
| turbo | turbo |  | `--turbo` |
| turbo |  | t | `-t` |
| turbo | turbo | t | `-t | --turbo` |
| t |  |  | `-t` |

## Flags

Flags are boolean options with a default value of false. The presence of the flag indicates true. This simplifies the user experience by allowing them to specifiy `-b` instead of `-b true`.  This also enables clubbing.

Define them as Options with BooleanMode = BooleanMode.Implicit. Implicit is the default defined for `AppSettings.DefaultBooleanMode` or `OptionAttribute.BooleanMode`.

## Flag Clubbing

Clubbing (aka bundling) is when several flags are specified together using their short names. 

For example: `-abc` is the same as `-a -b -c`

Clubbing is only available for flags by short name.

## Option assignments

When assigning option values, the following are the same

* `--time tomorrow`
* `--time=tomorrow`
* `--time:tomorrow`