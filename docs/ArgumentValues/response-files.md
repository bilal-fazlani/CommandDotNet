# Response Files

Response files are text files containing arguments that can be replaced by their contents in the command line.
Micrsoft uses them in several applications including [msbuild](https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-response-files?view=vs-2019) and the [MIDL compiler](https://docs.microsoft.com/en-us/windows/win32/midl/response-files). 

## TLDR, How to enable 
Enable the feature with `appRunner.UseResponseFiles()` or `appRunner.UseDefaultMiddleware()`.

## Example

``` c#
public void Interceptor(InterceptorExecutionDelegate next, string user, string pwd, string url)
{
    _client.Connect(user, pwd, url);
    return next();
}

public void Find(string filter, IConsole console)
{
    _client.Find(filter).ForEach(i => console.Out.WriteLine(i.Id));
}

public void Migrate(List<string> ids)
{
    ids.ForEach(_client.Migrate);
}
```

This app has 2 commands that would be called like this:

 * Find: `--user bob --pwd shhh -url dev.site.com Find isNew`
 * Migrate: `--user bob --pwd shhh -url dev.site.com Migrate 12 3 44`

The credentials could be stored in a file: creds/bobs-creds.rsp
``` ini
# bob's dev creds
--user bob --pwd shhhh
--url dev.site.com
```

Then the call to Find could be `@creds/bobs-creds.rsp Find isNew`.

The `expand-response-files` token transformation will replace `@creds/bobs-creds.rsp` with `--user bob --pwd shhhh --url dev.site.com`.

Rules:

* The file path must be prefixed with `@`
* Empty lines and lines starting with `#` are skipped
* Each line is run through a `CommandLineStringSplitter` to split arguments while honoring quoted strings
* Multiple arguments can be specified on a single line
* Arguments can be specified on multiple lines.
* Files can have any extension as long as the contents are text.

## Escaping the @

For cases where an option or operand require a value that begins with `@`. 

**Options**: Use `=` or `:` for the value assignment.  `--who:@me`.

**Operands**: Use the [argument separator](argument-separator.md). Any argument after the separator will not be evaluated as a response file.

## Recipes

There are a number of ways to use response files to improve usability and testiblity of your app.

### Inter-command communication

Similar to [piped arguments](../ArgumentValues/piped-arguments.md), a command can write arguments to a response file and that response file can be used by other commands. 
This can be used to "pipe" options to other commands.

### Fail File

An application can store ids of failed items in failed.txt file that can be used to rerun just the failed items.
For example:

``` c#
public void Migrate([Option] string failFilePath, List<string> ids)
{
    if(!ids.Any())
    {
        ids = _repo.GetAllIds();
    }
    foreach(var id in ids)
    {
        try
        {
            _client.Migrate(id);
        }
        catch(Exception ex)
        {
            Log.Error(ex);
            if(failFilePath != null)
            {
                File.AppendAllText(failFilePath, id + Environment.NewLine);
            }
        }
    }
}
```

First run: `Migrate --failFilePath failed1.txt` to migrate all items and capture failed ids to `failed1.txt`

Trouble shoot errors and deploy code with bug fixes.

Second run: `Migrate --failFilePath failed2.txt @failed1.txt` to migrate all items in `failed1.txt` and capture failed ids to `failed2.txt`

Rinse and repeat.

### Resume Session

A command that enumerates items can maintain a session file with arguments that allow resuming the proceess if cancelled or crashed.

For example:

``` c#
public void Migrate([Option] string session, List<string> ids)
{
    if(!ids.Any())
    {
        ids = _repo.GetAllIds();
    }
    foreach(var id in ids.ToList())
    {
        _client.Migrate(id);
        ids.RemoveAt(0);
        File.WriteAll($"{session}.rsp", $"session={session} ids={string.join(',', ids)}");
    }
}
```

Run: `Migrate --session mysession` to capture session data to `mysession.rsp`

`migrate.exe @mysession.rsp` to resume the operation. Since the session option is maintained, the session file will continue to update.
 
### Pinning commands

When commands (or options) are often repeated with the same values.

> `Migrations --env test --username lala migrate-orders --customer acme ...`

Compose fine-grained response files allowing to mix and match options

* @test.rsp > `--env test --username lala`
* @migrate-acme.rsp > `migrate-orders --customer acme`

> `Migrations @test.rsp @migrate-acme-orders.rsp ...`

This is an alternative to shell scripts, enabling mix & match for commonly used options.

This is also useful for test scripts.

### Testing

Response files can be used like configuration files. You can store them for different testing configurations and share them with your team.
This can be more efficient than creating batch scripts because you can change some options or in some cases, reuse the same response file across different commands. 