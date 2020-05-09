# Parse Directive

## TLDR, How to enable 
Enable the feature with `appRunner.UseParseDirective()` or `appRunner.UseDefaultMiddleware()`.

## Parse Directive

Rules for including spaces in arguments and escaping special characters differ from shell to shell. Sometimes it's not clear why arguments are not mapping correctly. This problem can be compounded when using [token transformations](../Extensibility/token-transformations.md) like [repsonse files](../ArgumentValues/response-files.md) where the arguments are not easily discoverable by simply logging the arguments passed to Program.Main(string[] args)

The `[parse]` [directive](../Extensibility/directives.md) will display the targetted command and each argument with values and the inputs and defaults used to derive the values.

```bash
~
$ dotnet example.dll [parse] LaunchRocket mars earth jupiter @flight-plan.rsp

command: LaunchRocket

arguments:

  planets <Text>
      value: mars, earth, jupiter, mercury, venus, saturn
      inputs:
        [argument] mars, earth, jupiter
        [piped stream]
      default: source=AppSetting key=--planets: mars, jupiter

options:
  
  crew <Text>
      value: Aaron, Alex
      inputs: [prompt] Aaron, Alex
      default: Bilal

  username <Text>
      value: Bilal
      inputs:
      default: source=EnvVar key=Username: Bilal
  
  password <Text>
      value: *****
      inputs: [prompt] *****
      default: source=EnvVar key=Password: *****
  
  turbo <Flag>
      value: true
      inputs: true (from: @flight-plan.rsp -> -ts -> -t)
      default: false
  
  slingshot <Flag>
      value: true
      inputs: true (from: @flight-plan.rsp -> -ts -> -s)
      default: false
```

There are several things to notice here:

* Multiple input sources can be displayed. (planets)
* Defaults from [configs and env vars](../ArgumentValues/default-values-from-config.md) are supported. (planets, username, password)
* [Passwords](../Arguments/passwords.md) are obscured.
* Token transformations are unwrapped, making it possible to indentify exactly how an argument was provided, including any response files used. (turbo, slingshot)

To provide this mapping, the middleware pipeline must progress to the BindValues stage. 

If that stage is not reached, the parse directive will fall back to printing just the token transformations.

## Token Transformations

!!!Warning
    Passwords entered from the shell (not via prompts) can be exposed in token transformations if CommandDotNet was not able to map them to an argument and determine they should be obscured. 

```bash
~
$ dotnet example.dll [parse] LaunchRocket mars earth jupiter @flight-plan.rsp

Unable to map tokens to arguments. Falling back to token transformations.

>>> from shell
  Directive: [parse]
  Value    : LaunchBigRocket
  Value    : mars
  Value    : earth
  Value    : jupiter
  Value    : @flight-plan.rsp
>>> after: expand-response-files
  Directive: [parse]
  Value    : LaunchBigRocket
  Value    : mars
  Value    : earth
  Value    : jupiter
  Value    : @flight-plan.rsp
  Option   : -ts
>>> after: expand-clubbed-flags
  Directive: [parse]
  Value    : LaunchBigRocket
  Value    : mars
  Value    : earth
  Value    : jupiter
  Value    : @flight-plan.rsp
  Option   : -t
  Option   : -s
>>> after: split-option-assignment
  Directive: [parse]
  Value    : LaunchBigRocket
  Value    : mars
  Value    : earth
  Value    : jupiter
  Value    : @flight-plan.rsp
  Option   : -t
  Option   : -s
```

These transformations can be appended to the regular parse output with `[parse:t]`