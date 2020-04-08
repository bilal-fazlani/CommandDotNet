# Getting Started

## Installation

CommandDotNet can be installed from [nuget.org](https://www.nuget.org/packages/CommandDotNet/)

=== ".NET CLI"

    ```
    dotnet add package CommandDotNet
    ```


=== "Nuget Package Manager"

    ```
    Install-Package CommandDotNet
    ```
    
## Let's build a calculator

Let's say you want to create a calculator console application which can perform 2 operations:

1. Addition
2. Subtraction

It prints the results on console.

Let's begin with creating the class

```c#
public class Calculator
{
    public void Add(int value1, int value2)
    {
        Console.WriteLine($"Answer:  {value1 + value2}");
    }

    public void Subtract(int value1, int value2)
    {
        Console.WriteLine($"Answer:  {value1 - value2}");
    }
}
```

Now that we have our calculator ready, let's see about how we can call it from command line.


```c#
class Program
{
    static int Main(string[] args)
    {
        return new AppRunner<Calculator>().Run(args);
    }
}
```

Assuming our application's name is `example.dll`

let's try and run this app from command line using dotnet

INPUT

```bash
dotnet example.dll --help
```

OUTPUT

```bash
Usage: dotnet example.dll [command]

Commands:

  Add
  Subtract

Use "dotnet example.dll [command] --help" for more information about a command.

```

Voila!

So, as you might have already guessed, it is detecting methods of the calculator class. How about adding some helpful description.

```c#
[Command(Description = "Adds two numbers")]
public void Add(int value1, int value2)
{
    Console.WriteLine($"Answer: {value1 + value2}");
}
```

This should do it.

Let's see how the help appears now.

```bash
Usage: dotnet example.dll [command]

Commands:

  Add       Adds two numbers. duh!
  Subtract

Use "dotnet example.dll [command] --help" for more information about a command.

```

Awesome. Descriptions are not required but can be very useful depending upon the complexity of your app and the audience. 

Now let's see if we can get further help for the add command.

INPUT

```bash
dotnet example.dll Add --help
```

OUTPUT

```bash
Adds two numbers. duh!

Usage: dotnet example.dll Add [arguments]

Arguments:

  value1  <NUMBER>

  value2  <NUMBER>
```

tada!

Ok, so here, it show what parameters are required for addition and their type.

Let's try and add two numbers.

INPUT

```bash
dotnet example.dll Add 40 20
```

OUTPUT

```bash
Answer: 60
```

## Opt-In to additional features

In the `Program.Main`, we configured the app with the basic feature set.
```c#
    return new AppRunner<Calculator>().Run(args);
```

To take advantage of many more additional features, such as
[debug](debug-directive.md) & [parse](parse-directive) directives,
[ctrl+c support](cancellation.md),
[prompting](prompting.md),
[piping](piped-arguments.md),
[response files](response-files.md) and [typo suggestions](typo-suggestions.md), add `UseDefaultMiddleware()`

```c#
    return new AppRunner<Calculator>()
        .UseDefaultMiddleware()
        .Run(args);
```

see [Default Middleware](default-middleware.md) for more details and options for using default middleware.

## Next Steps

You get the gist of this library now. This may be all you need to start your app.

Check out the

* *Defining Commands* section to learn more details around defining commands, subcommands and arguments.

* *Middleware* section to see the additional set of features available via middleware.

* *Extensibility* section if the framework is missing a feature you need and you're interested in adding it yourself. For questions, ping us on our [Discord channel](https://discord.gg/QFxKSeG) or create a [GitHub Issue](https://github.com/bilal-fazlani/commanddotnet/issues)

* *Diagnostics* section for a rich set of tools to simplify troubleshooting

* *Test Tools* for a test package to help test console output with this framework. This is helpful for all apps, but especially helpful when definiing extensibility components. This package is used to test all of the CommandDotNet features.