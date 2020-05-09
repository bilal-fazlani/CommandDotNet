# Arity & Default Values

## Arity

[Arity](https://en.wikipedia.org/wiki/Arity) describes how many values a user can or must provide for an argument.

`ArgumentArity` is expressed as a _minimum_ and _maximum_ value 

* shorthand: `{min}..{max}`. eg. `0..1` min=0 max=1

When...

  * _minimum_ == 0, no values are required 
      * this applies to flags and [optional arguments](#optional-arguments)
  *  _maximum_ > 1, multiple values can be provided
  * _maximum_ == int.MaxValue, an unlimited number of values can be provided

!!! note
    There is currently no validation for arity and no way to set the arity via attributes. 
    but is [on the roadmap](https://github.com/bilal-fazlani/commanddotnet/issues/195). 
    
    Until then, arity validation is left to developers to define custom middleware.

`ArgumentArity` contains the following members to encapsulate the common use cases. 

```c#
public static readonly int Unlimited = int.MaxValue;

public static IArgumentArity Zero => new ArgumentArity(0, 0);
public static IArgumentArity ZeroOrOne => new ArgumentArity(0, 1);
public static IArgumentArity ExactlyOne => new ArgumentArity(1, 1);
public static IArgumentArity ZeroOrMore => new ArgumentArity(0, Unlimited);
public static IArgumentArity OneOrMore => new ArgumentArity(1, Unlimited);

public bool AllowsNone() => Maximum == 0;
public bool AllowsMany() => Maximum > 1;
public bool RequiresAtLeastOne() => Minimum > 0;
public bool RequiresExactlyOne() => Minimum == 1 && Maximum == 1;
```

The static method `ArgumentArity.Default(type, ...)` will return one of these static values based on the type

### Possible Arities

|            |none (flags)|optional|required     |
|---         |---         |---     |---          |
|single value|0..0        |0..1    |1..1         |
|list values |--          |0..N(>0)|M(>0)..N(>=M)|

* arity for single value can be
     * 0..0 `Zero`
     * 0..1 `ZeroOrOne` (optional)
     * 1..1 `ExactlyOne` (required)
* arity for list values can be
    * 0..N(>0) `ZeroOrMore` (optional) N must be greater than 0
    * M(>0)..N(>=M) `OneOrMore` (required) N must be greater than or equal to M. 
        * If N equal M, the list must have exactly M values.

## Optional Arguments 

An argument is considered optional when defined as...

* a Nullable<T> type  eg. bool?, Guid?
* an optional parameter
* an `IArgumentModel` property with a default value where the default value != default(T)
    * the value must be set in the ctor or property assignment. This condition is evaluated immediately after instantiation.
    * a null property can never be assumed to have a _minimum_ arity of 0. Reference type properties will always be considered required.

## Default Values

Middleware can set default values for arguments. Setting default values will __not__ modify the arity.

Consider there are two categories of default values

1. Statically defined by the application developer, such as described in the [optional arguments](#optional-arguments) section. 
    * These can be inferred to mean the user is not required to enter a value. 
    * This could also be defined by an attribute or some other static mechanism.
1. Default values specified by the user to simplify use of the application. For example, pulling default values from [Environment Variables and AppSettings](../ArgumentValues/default-values-from-config.md). 
    * The application requires a value but the user does not have to enter it because it is defaulted by a middleware component.

Any middleware updating the default values according to the first category should also update the arity. 
Use `argument.Arity = ArgumentArity.Default(argument)` to calculate a new arity for the argument.

## Summary by definition type

|defined using|defaults|optional| 
|---          |---    |---        |
|parameter    |optional parameters | when Nullable or parameter is optional |
|property     |property value immediately after initialization | |
| -- struct   |       | when Nullable or default != default(T) | 
| -- class    |       | when default != null | 

Arguments defined as parameters have consistent behavior for struct and class types.

Arguments defined as properties have have differing behavior between struct and class types.