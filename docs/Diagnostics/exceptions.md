# Exceptions

CommandDotNet rethrows all exceptions raised by the application code. 

There are two patterns for handling exceptions

=== "HandleErrorDeletage"

    Register an ErrorHandler middleware method using MiddlewareSteps.ErrorHandler
    to ensure the middleware is one of the last in the pipeline to exit.

    ```c#
    public static AppRunner GetRunner()
    {
        return new AppRunner<Calculator>()
                    .UseErrorHandler((ctx, ex) =>
                    {
                        ctx.Console.Error.WriteLine(ex.Message);
                        return 1;
                    });
                    .Run(args);
    }
    ```

=== "Wrap AppRunner.Run"

    Wrap the call to `appRunner.Run(args)` to handle global exceptions.

    ```c#
    var appRunner = new AppRunner<Calculator>();
    try
    {
        return appRunner.Run(args);
    }
    catch(MyBusinessException ex)
    {
        Console.Error.WriteLine(ex.Message);
    }
    ```

!!! Tip
    Use the HandleErrorDeletage if you plan to test your app using the [TestTools](../TestTools/overview.md) approach [mentioned here](../TestTools/overview.md#testing-your-application)

CommandDotNet internal errors are generally captured and return error code 1, using `ExitCodes.Error`.

Validation middleware will generally return error code 2, using `ExitCodes.ValidationError`.

Middlware should print error messages and return an error code instead of raising exceptions.

## Additional Diagnostic Info

### CommandContext from the exception

Exceptions that escape appRunner will contain the CommandContext in the Exception.Data property.

Use the `ex.GetCommandContext()` extension method to get the instance. Use the namespace `CommandDotNet.Diagnostics`

With the context, you can `.PrintHelp()` or `ParserReport.Report` to log details of the request.

### Printing config information

Configuration can be printed with `appRunner.ToString()`.  See the [Command Logger > AppConfig](command-logger.md#appconfig) section for an example.

Alternatively, `CommandLogger.Log(ctx)` can be used to output CommandLogger for exception handling.

### Full example

```c#
var appRunner = new AppRunner<Calculator>();
try
{
    return appRunner.Run(args);
}
catch(MyBusinessException ex)
{
    Console.Error.WriteLine(ex.Message);

    var ctx = ex.GetCommandContext();

    // use CommandLogger if it has not already logged for this CommandContext
    if(!CommandLogger.HasLoggedFor(ctx))
    {
        CommandLogger.Log(
            ctx, 
            includeSystemInfo: true,
            includeAppConfig: true
        )
    }

    // print help for the target command
    // will be empty if the exception occurred before
    // a command could be parsed.
    Console.Out.WriteLine(ctx.PrintHelp());
}
```