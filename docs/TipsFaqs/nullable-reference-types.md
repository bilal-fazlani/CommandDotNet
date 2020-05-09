# Nullable Reference Types

If you've enabled Nullable Reference Type checks in your solution, you'll start getting a notice that "Non-nullable property {name} is uninitialized" for properties

* properties decorated with [SubCommand] to define a subcommand
* class type properties in argument models
* class type properties in command classes that are initialized from an interceptor or command method

This same problem occurs for DTOs populated by serializers and ORMs. 
We can follow the same recommendation by [EF Core](https://docs.microsoft.com/en-us/ef/core/miscellaneous/nullable-reference-types#dbcontext-and-dbset).

```c#
public void MyCommmandClass
{
    [SubCommand]
    public MySubcommandClass Subcommand{ get; set; } = null!;
    
    // populated by interceptor method
    public FileInfo _inputs = null!;
    
    public Task<int> Interceptor([Option] FileInfo inputs)
    {
        _inputs = inputs ?? throw new ArgumentNullException(nameof(inputs));
    }
}

public class ArgModel : IArgumentModel
{
    public FileInfo Outputs { get; set; } = null!;
}
```

Obviously, you should only use `= null!` when you can guarantee code won't access the properties while they are null.