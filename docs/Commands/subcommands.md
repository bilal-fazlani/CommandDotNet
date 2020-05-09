# Subcommands

You can nest commands. Let's take `git` for example

`git` has a command called stash. When you execute `git stash`, it stashes all the changes. But stash has further commands like, `git stash pop`, `git stash list`, etc.

Let's mimic the same behavior using CommandDotNet:

## Subcommand as a property

```c#
[Command(Description = "Fake git application")]
public class Git
{
    // Properties decorated with the [SubCommand] attribute
    // will be registered as subcommands.
    [SubCommand]
    public Stash Stash {get;set;}

    [Command(Description = "Commits all staged changes")]
    public void Commit([Option(ShortName = "m")]string commitMessage)
    {
        Console.WriteLine("Commit successful");
    }
}
```
```c#
[Command(Description = "Stashes all changes when executed without any arguments")]
public class Stash
{
    [DefaultMethod]
    public void StashDefaultCommand()
    {
        Console.WriteLine($"changes stashed");
    }

    [Command(Description = "Applies last stashed changes")]
    public void Pop()
    {
        Console.WriteLine($"stash popped");
    }

    [Command(Description = "Lists all stashed changes")]
    public void List()
    {
        Console.WriteLine($"here's the list of stash");
    }
}
```
Notice the `Stash` property decorated with the `[SubCommand]` attribute.

The help will generate as:

```bash
Fake git application

Usage: git.exe [command] [options]

Commands:
  Commit  Commits all staged changes
  Stash   Stashes all changes when executed without any arguments

Use "git.exe [command] --help" for more information about a command.
```

Here's how the interaction looks like:

```bash
~
$ git.exe commit -m "some refactoring"

Commit successful

~
$ git.exe stash

changes stashed

~
$ git.exe stash --help

Stashes all changes when executed without any arguments

Usage: git.exe Stash [command] [options]

Commands:
  List  Lists all saved stashed changes
  Pop   Applies last stashed changes

~
$ git.exe stash pop

stash popped
```

!!! Tip
    See [Nullable Reference Types](../TipsFaqs/nullable-reference-types.md) for avoiding  "Non-nullable property is uninitialized" warnings for subcommand properties

## Subcommand as a nested class

The same git stash command could be modelled as a nested class.

```c#
[Command(Description = "Fake git application")]
public class Git
{
    [Command(Description = "Commits all staged changes")]
    public void Commit([Option(ShortName = "m")]string commitMessage)
    {
        Console.WriteLine("Commit successful");
    }

    // Nested classes decorated with the [SubCommand] attribute
    // will also be registered as subcommands.
    [SubCommand]
    [Command(Description = "Stashes all changes when executed without any arguments")]
    public class Stash
    {
        [DefaultMethod]
        public void StashDefaultCommand()
        {
            Console.WriteLine($"changes stashed");
        }
    
        [Command(Description = "Applies last stashed changes")]
        public void Pop()
        {
            Console.WriteLine($"stash popped");
        }

        [Command(Description = "Lists all stashed changes")]
        public void List()
        {
            Console.WriteLine($"here's the list of stash");
        }
    }
}
```