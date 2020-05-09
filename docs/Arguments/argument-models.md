# Argument Models
Argument models provide a way to define arguments in a class.

This command to send an e-mail...

```bash
send --subject hi -a "myFile.txt" --body "just wanted you to review these files" bilal@bilal.com john@john.com
```

can be defined with a method as ...

```c#
public void SendEmail(
    [Option]string subject, 
    [Option]string body, 
    [Operand]string from, 
    [Operand]string to)
{

}
```

or with an argument model as ...

```c#
public class Email : IArgumentModel
{
    [Option]
    public string Subject { get; set; }
    
    [Option]
    public string Body { get; set; }
    
    [Operand]
    public string From { get; set; }
    
    [Operand]
    public string To { get; set; }
}

public void SendEmail(Email email)
{

}
```

## Composition

An `IArgumentModel` can be composed from other `IArgumentModel`s allowing easy reuse of common arguments.

```c#
public class DryRun : IArgumentModel
{    
    [Option(LongName="dryrun")]
    public bool IsDryRun { get; set; }
}

public class SendEmailArgs : IArgumentModel
{    
    public DryRun DryRun { get; set; }
    
    public Email Email { get; set; }
}

public void SendEmail(SendEmailArgs args)
{

}
```

## Benefits of argument models

* Common arguments can be extracted to models to enforce behaviors across commands. This ensures the same short name, long name, description, etc are consistent across all commands using this model.
* A [middleware](../Extensibility/middleware.md) could be created to cancel a UnitOfWork when a dry-run is requested.
* [FluentValidation](../Arguments/fluent-validation-for-argument-models.md) framework can be used to validate the model.

Take `DryRun` for example. Ask 5 different developers to add a dryrun option and you'll end up with 5 different casings for it. Add it to an IArgumentModel and everyone can use and the commands will have a consistent argument.  

When you have the same model, you can add middleware that can check for the existing of that model and perform logic based on that.  Using the `DryRun` example, a UnitOfWork middleware could determine whether to commit or abort a transaction based on the value of the model.

!!! Tip
    See [Nullable Reference Types](../TipsFaqs/nullable-reference-types.md) for avoiding  "Non-nullable property is uninitialized" warnings in your argument models

## Guaranteeing the order of arguments

Prior to version 4, argument position is not guaranteed to be consistent because the .Net Framework does not guarantee the order properties are reflected.

> The [GetProperties](https://docs.microsoft.com/en-us/dotnet/api/system.type.getproperties) method does not return properties in a particular order, such as alphabetical or declaration order. Order can differ on each machine the app is deployed to. Your code must not depend on the order in which properties are returned because that order is no guaranteed.

For `Operands`, which are positional arguments, this can result in commands with operands in a non-deterministic order.

This is less of an issue with `Option` because options are named, not positional. Only the order options appear in help is affected.

As of version 4, CommandDotNet can guarantee all arguments will maintain their position as defined within a class as long as the properties are decorated with `OperandAttribute`, `OptionAtribute` or `OrderByPositionInClassAttribute`.

### How to use

The `OperandAttribute` and `OptionAtribute` define an optional constructor parameter called `__callerLineNumber`. This uses the [CallerLineNumberAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.callerlinenumberattribute?view=netframework-4.8) to auto-assign the line number in the class. **Do Not** provide a value for this parameter.

CommandDotNet will raise an exception when the order cannot be determined.

Order cannot be determined when

1. `AppSettings.DefaultArgumentMode == ArgumentMode.Operand` (the default) and the property is not attributed with `[Operand]`
1. When a nested argument model containing an operand is not decorated with `[OrderByPositionInClass]`

When the guarantee is enabled, our SendEmail example above will fail with 
  > `Operand property must be attributed with OperandAttribute or OrderByPositionInClassAttribute to guarantee consistent order. Property: ExampleApp.SendEmailArgs.Email`

We can fix by attributing the `Email` property like so...

```c#
public class SendEmailArgs : IArgumentModel
{    
    public DryRun DryRun { get; set; }
    
    [OrderByPositionInClass]
    public Email Email { get; set; }
}
```
 
### Recommendation 
* When possible, do not define operands in nested argument models.
* Always attribute operands in properties with `[Operand]`.