<img src="./images/logo.png" width="250px" />

# CommandDotNet

![Nuget](https://img.shields.io/nuget/v/commanddotnet?style=for-the-badge)
[![NuGet Pre Release](https://img.shields.io/nuget/vpre/CommandDotNet.svg?style=for-the-badge)](https://www.nuget.org/packages/CommandDotNet)
[![NuGet](https://img.shields.io/nuget/dt/CommandDotNet.svg?style=for-the-badge)](https://www.nuget.org/packages/CommandDotNet)
[![GitHub](https://img.shields.io/github/license/bilal-fazlani/commanddotnet?style=for-the-badge)](https://github.com/bilal-fazlani/commanddotnet/blob/master/LICENSE)
![Netlify](https://img.shields.io/netlify/11a3718e-f340-4c65-81e2-9c49b6ebb842?label=Netlify%20Build&style=for-the-badge)


[![GitHub last commit](https://img.shields.io/github/last-commit/bilal-fazlani/CommandDotNet.svg?style=for-the-badge)]()
![Travis (.com) branch](https://img.shields.io/travis/com/bilal-fazlani/commanddotnet/master?label=TRAVIS%20BUILD&style=for-the-badge)

[![Gitter](https://img.shields.io/gitter/room/badges/shields.svg?style=for-the-badge)](https://gitter.im/CommandDotNet/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)
[![Discord](https://img.shields.io/discord/678568687556493322?label=Discord%20Chat&style=for-the-badge)](https://discord.gg/QFxKSeG)

---

## Documentation

Version 3 (Current): https://commanddotnet.bilal-fazlani.com

Version 2 (Previous): https://v2.commanddotnet.bilal-fazlani.com

***Model your command line application interface in a class***

The purpose of this **framework** is to let developers focus on the core logic of command line application by defining commands with methods and arguments with parameters. 

*Out-of-the-box* support for help documentation, subcommmands, dependency injection, validation, piping, prompting, passwords, response files and more. 

Includes [test tools](https://commanddotnet.bilal-fazlani.com/test-tools) used by the framework to test all features of the framework.

Modify and extend the functionality of the framework through configuration and middleware.

### Example

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

```c#
class Program
{
    static int Main(string[] args)
    {
        return new AppRunner<Calculator>().Run(args);
    }
}
```

With these two classes, we've defined an Add and Subtract command and help is automatically generated.

```bash
~
$ dotnet calc.dll -h
Usage: dotnet calc.dll

Usage: dotnet calc.dll [command]

Commands:

  Add
  Subtract

Use "dotnet calc.dll [command] --help" for more information about a command.

~
$ dotnet calc.dll Add -h
Usage: dotnet calc.dll Add [arguments]

Arguments:

  value1  <NUMBER>

  value2  <NUMBER>

~
$ dotnet calc.dll Add 40 20
Answer: 60
```

Check out the docs for more examples

## Credits 🎉

Special thanks to [Drew Burlingame](https://github.com/drewburlingame) for continuous support and contributions
