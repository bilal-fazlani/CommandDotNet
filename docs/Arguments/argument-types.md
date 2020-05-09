# Supported Argument Types

Arguments can be defined with any type that...

* is a primitive type: 
* contains a string constructor
* has a TypeConverter

Includes, but not limited to:

- `string`,`char`,`enum`,`bool`
- `short`,`int`,`long`,`decimal`,`double`
- `Guid`,`Uri`,`FileInfo`,`DirectoryInfo`

Also supports `Nullable<T>` and `IEnumerable<T>` (`T[]`, `List<T>`, etc.) where T can be converted from string.

!!! Note 
    There can be only one `List` operand in the method, where `List` is any type of `IEnumerable`. 
    `List` operands must be the last operand defined for the method.

## Adding support for other types

Options for supporting other types

* If you control the type, consider adding a constructor with a single string parameter.
* Create a [TypeConverter](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.typeconverter?view=netframework-4.8) for your type
* Create a [type descriptor](#type-descriptors)

### Type Descriptors

Type descriptors are your best choice when you need 

- to override an existing TypeConverter
- conditional logic based on argument metadata (custom attributes, etc)
- the converter only for parsing parameters and not the business logic of your application

Implement [IArgumentTypeDescriptor](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/TypeDescriptors/IArgumentTypeDescriptor.cs) or instantiate a [DelegatedTypeDescriptor<T>](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/TypeDescriptors/DelegatedTypeDescriptor.cs) and register with `AppSettings.ArgumentTypeDescriptors.Register(...)`.

See [StringCtorTypeDescriptor](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/TypeDescriptors/StringCtorTypeDescriptor.cs) and [ComponentModelTypeDescriptor](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/TypeDescriptors/ComponentModelTypeDescriptor.cs) for examples.

If the type has a limited range of acceptable values, the descriptor should also implement [IAllowedValuesTypeDescriptor](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/TypeDescriptors/IAllowedValuesTypeDescriptor.cs).  See [EnumTypeDescriptor](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/TypeDescriptors/EnumTypeDescriptor.cs) for an example.