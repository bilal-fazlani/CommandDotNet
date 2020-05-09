# Option vs Operand

It is not always clear when to use an option vs operand. Below are a couple of strategies to help.

## Every argument an option. 

Set `AppSettings.DefaultArgumentMode` to `ArgumentMode.Option` and treat every argument as an option. 

Advantages

* User cannot accidently provide arguments in the wrong order
* Adding arguments is less likely to introduce breaking changes
* If the console input is logged, it's easier to determine user intent at a glance. Especially for older versions of the command with different arguments

Disadvantages

* More typing for the user. Imagine if you had to type argument names for all git commands... 
    * `git checkout --branch branch-name` instead of `git checkout branch-name` 
    * `git push --remote origin --branch branch-name` instead of `git push origin branch-name`

Consider how often users will be using these commands. If use will generally be from scripts and automation, use all options for backward compatibility.

## Operand as "what", Option as "how"
This approach can lead to a more elegant design of your commands. 

Consider [operands as *"what"*](https://en.wikipedia.org/wiki/Operand) the command operates on and options as *"how"* the command operates on them, as described in [this article](http://www.informit.com/articles/article.aspx?p=175771).

In the example below, `x` and `y` are the operands and `--radix` informs how the numbers are represented in the operations. 

``` c#
public void Add(int x, int y, [Option] int? radix)
{
    Console.WriteLine(_calculator.Add(x, y, radix));
}
```

The exercise of distinguishing *what* vs *how* can bring clarity to your separation of concerns, leading to a cleaner interface.
e.g. Identifying `--radix` as a *"how"* makes it an obvious candidate to promote to a parent command as shown in [interceptors](../Extensibility/interceptors.md) example and then it is available for other math operations.

## Optional Operands

Optional operands are a muddy topic. As a general rule, for simplicity, define them as options on the command. This is especially true if there are multiple. 

This framework will assign positional arguments in the order they are specified in the shell so if a user skipped the first optional operand and provided a value for the second one, the framework would still assign the value to the first one because it cannot understand the users intent.

By defining these as options, users can specify intent with the name. This is similar to how C# uses optional parameters.