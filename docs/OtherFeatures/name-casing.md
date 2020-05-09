# Name Casing

Use [Humanizer](https://github.com/Humanizr/Humanizer) with CommandDotNet to convert the case of command and arguments to PascalCase, camelCase, lowercase or kebab-case.

## TLDR, How to enable 

=== ".NET CLI"

    ```
    dotnet add package CommandDotNet.NameCasing
    ```
    
=== "Nuget Package Manager"

    ```
    Install-Package CommandDotNet.NameCasing
    ```
Enable the feature with `appRunner.NameCasing(...)`

## Case options

```c#
public class App
{
    public void MigrateUser([Option]bool dryRun){...}
}
```

With `Case.DontChange`, the command is executed as `MigrateUser --dryRun`

With `Case.PascalCase`, the command is executed as `MigrateUser --DryRun`

With `Case.CamelCase`, the command is executed as `migrateUser --dryRun`

With `Case.KebabCase`, the command is executed as `migrate-user --dry-run`

With `Case.LowerCase`, the command is executed as `migrateuser --dryrun`

## Overridden names

By default, the case is only applied where the name has not been overridden in an attribute.

```c#
public class App
{
    [Command(Name="migrateUser")]
    public void MigrateUser([Option(LongName="DryRun")]bool dryRun){...}
}
```

the command is executed as `MigrateUser --DryRun`.

Use the `applyToNameOverrides` option to apply case conversion to migrateUser and DryRun. 

!!!Caveats
    Humanizer behavior

    * Lowercase cannot be converted to another case... except, the first letter will be capitalized for Pascal. Humanizer doesn't know where the second word starts.
    * Kebabcase cannot be converted to camelcase or lowercase. No idea why.
    * Camel and Pascal can be converted to any other case

## Custom Name Transforamtions

use `appRunner.Configure(c => c.NameTransformation = ...)` to apply custom name transformations.

Example: [Humanizer middleare](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.NameCasing/HumanizerAppRunnerExtensions.cs)

This example will lowercase all names

```c#
appRunner.Configure.NameTransformation = 
    (attributes, memberName, nameOverride, commandNodeType) 
        => (nameOverride ?? memberName).ToLower()`
```
