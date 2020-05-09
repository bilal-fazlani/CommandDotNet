# FluentValidation

Use [FluentValidation](https://github.com/JeremySkinner/FluentValidation) with CommandDotNet to validate [argument models](../Arguments/argument-models.md).

## TLDR, How to enable 

=== ".NET CLI"

    ```
    dotnet add package CommandDotNet.FluentValidation
    ```
    
=== "Nuget Package Manager"

    ```
    Install-Package CommandDotNet.FluentValidation
    ```

Enable with `appRunner.UseFluentValidation()`, or `appRunner.UseFluentValidation(showHelpOnError: true)` <br/> to print help when there are validation errors.

## Example
```c#
class Program
{
    static int Main(string[] args)
    {
        return new AppRunner<ValidationApp>()
            .UseFluentValidation()
            .Run(args);
    }
}

public class ValidationApp
{
    public void ValidateModel(PersonModel person)
    {
        string content = JsonConvert.SerializeObject(person, Formatting.Indented);
        Console.WriteLine(content);
    }
}

[Validator(typeof(PersonValidator))]
public class PersonModel : IArgumentModel
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Email { get; set; }
}

public class PersonValidator : AbstractValidator<PersonModel>
{
    public PersonValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
```

If the validation fails, app exits with return code 2 and outputs validation error messages to error output.