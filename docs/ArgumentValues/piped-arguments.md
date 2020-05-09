# Piped arguments

## TLDR, How to enable 
Enable the feature with `appRunner.AppendPipedInputToOperandList()` or `appRunner.UseDefaultMiddleware()`.

## What is piping?
Piping is a way to pass the console output of a command to another command.

A simple example is filtering your history of bash commands for git commands: `history | grep git`.

The pipe `|` tells the shell you are running a new command and that the new command will take the output of the previous command.  
In this case, `history` will output the last N bash commands and `grep git` will filter the output to only the commands containing "git"

Piping is a great way to decompose commands into smaller, single-responsibility commands.

!!!tip
    When you find commands repeating the same options to determine what items to operate on, consider extracting the options into search command that returns ids and then update the other commands take the list of ids. 

    `app.exe get-customer-ids --only-active | app.exe migrate-customers`

In .net console apps, you can check piped input using `Console.IsInputRedirected` and `Console.In.Read___()`. 

Or... use the `AppendPipedInputToOperandList` middleware

## Using the middleware

Every command is allowed a single operand list. If one is defined and if piped input is available, it will be appended to this list. If the user pipes input and also specifies values for the operand, the two sources will be concatenated with piped input at the end.

#### Example

Given the commands defined below and this input in the shell: `Users Find -a | Users Disable`

The shell will start two processes, the first for Find and the second for Disable. As the first process writes ids to the output stream, the id is "piped" to the second process. The Disable command will process the ids as they are received. When Find completes, the first process will shutdown and the pipe will be closed. The Disable command will complete any remaining ids and then shutdown.

If Disable used a type of `List<string>` or `string[]`, then Disable could not process the ids until the Find process completed. By using `IEnumerable<string>` results can be streamed.

``` c#
public class Users
{
    public void Find(
        IConsole console,
        [Option(ShortName="a")] bool activeOnly, 
        string pattern = null)
    {
        foreach(var user in userSvc.GetUsers(activeOnly, pattern))
        {
            console.Out.WriteLine(user.Id)
        }
    }

    public void Disable(IEnumerable<string> ids)
    {
        foreach(string id in ids)
        {
            userSvc.Disable(id);
        }    
    }
}
```

!!!warn
    All output to the console is piped to the next command. Be sure the command generating the piped output does not include additional logging information.

You can experiment with this using our [pipes example command](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Example/Commands/Pipes.cs).

!!!tip
    When the collection type is streamable, like `IEnumerable<T>`, the values will be streamed to the enumerable as they're received from the publishing program,
    otherwise the command will have to wait until the publishing program is complete before it can begin processing.

 