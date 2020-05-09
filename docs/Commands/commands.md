# Commands

Commands are be defined by methods and classes.

Using our calculator example...

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

class Program
{
    static int Main(string[] args)
    {
        return new AppRunner<Calculator>().Run(args);
    }
}
```

__Calculator__ is the root command and is not directly referenced in the terminal.

__Add__ and __Subtract__ are the commands. 

Command methods must:

* be `public`
* return `void`, `int`, `Task` or `Task<int>`

Command methods may be async.

When the return type is `int` the value is used as the exit code.

## Command Attribute

Every public method will be interpreted as a command and the command name will be the method name.

Use the `[Command]` attribute to change the command name, enhance help output and provide parser hints.

```c#
public class Calculator
{
    [Command(Name="sum",
        Usage="sum <int> <int>",
        Description="sums two numbers",
        ExtendedHelpText="more details and examples")]
    public void Add(int value1, int value2)
    {
        Console.WriteLine($"Answer:  {value1 + value2}");
    }
}
```

```bash
~
$ dotnet add.dll Add --help

dotnet example.dll sum -h
sums two numbers

Usage: sum <int> <int>

Arguments:

  value1  <NUMBER>

  value2  <NUMBER>

more details and examples

```

Use `IgnoreUnexpectedOperands` & `ArgumentSeparatorStrategy` to override argument parsing behavior for the command. See [Argument Separator](../ArgumentValues/argument-separator.md) for more details.

## Default Method

Let's refactor our calculator. Let's rename the application to Add with the single Add command.

```c#
public class Calculator
{
    public void Add(int value1, int value2)
    {
        Console.WriteLine($"Answer:  {value1 + value2}");
    }
}
```

Now executed as

```bash
dotnet add.dll Add 1 2
```

Notice the redundant `Add` command. Fix this with the `[DefaultMethod]` attribute.

```c#
public class Calculator
{
    [DefaultMethod]
    public void Add(int value1, int value2)
    {
        Console.WriteLine($"Answer:  {value1 + value2}");
    }
}
```

Now executed as

```bash
dotnet add.dll 1 2
```
