# Features

|Feature| |
|---|---:|
|   **Commands** |  |
| \ \ \ Define commands as methods | [docs](Commands/commands.md) ✔️ |
| \ \ \ \ \ \ Parameter Resolvers<br/>\ \ \ \ \ \ \ \ \ _Inject console and other contexts as a parameter:_<br/>\ \ \ \ \ \ \ \ \ _IConsole, CommandContext, CancellationToken & IPrompter by default._ | [docs](Extensibility/parameter-resolvers.md) ✔️ |
| \ \ \ Infinite nesting of subcommands | [docs](Commands/subcommands.md) ✔️ |
| \ \ \ \ \ \ Nested classes | ✔️ |  |
| \ \ \ \ \ \ Composed classes | ✔️ |  |
| \ \ \ Command interception <br/>\ \ \ \ \ \ _[interceptors](Extensibility/interceptors.md) for subcommands <br/>\ \ \ \ \ \ [middleware](Extensibility/middleware.md) for all commands_ | ✔️ |
| \ \ \ Method interception <br/>\ \ \ \ \ \ _`CommandContext.InvocationPipeline` to access method and params_ | ✔️ |
|   **Arguments** |  |
| \ \ \ Positional (Operands) | [docs](Arguments/arguments.md) ✔️  |
| \ \ \ Named (Options) | [docs](Arguments/arguments.md) ✔️ |
| \ \ \ \ \ \ short and long names:  _`-h` or  `--help`, **not** /help_ | ✔️ |
| \ \ \ \ \ \ flags:  _`-a` instead of `-a true`_| [docs](Arguments/arguments.md#flags) ✔️ |
| \ \ \ \ \ \ bundling/clubbing: _`-a -b -c` or `-abc`_ | [docs](Arguments/arguments.md#flag-clubbing) ✔️ |
| \ \ \ \ \ \ value assignments: _`-a one`, `-a=one` & `-a:one`_ | [docs](Arguments/arguments.md#option-assignments) ✔️ |
| \ \ \ Define arguments as parameters in methods | [docs](Arguments/arguments.md) ✔️ |
| \ \ \ Define arguments as properties in POCOs | [docs](Arguments/argument-models.md) ✔️ |
|   **Data Types** |  |
| \ \ \ Primitives, Enums & Nullable< T > | [docs](Arguments/argument-types.md) ✔️ |
| \ \ \ Collections | [docs](Arguments/argument-collections.md) ✔️ |
| \ \ \ Dictionaries | [#251](https://github.com/bilal-fazlani/commanddotnet/issues/251) ❌ |
| \ \ \ Password: _mask value in logs and output with `*****`_ | [docs](Arguments/passwords.md) ✔️ |
| \ \ \ Any type with a string constructor | [docs](Arguments/argument-types.md#adding-support-for-other-types) ✔️ |
| \ \ \ Any type with a [TypeConverter](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.typeconverter?view=netframework-4.8) | [docs](Arguments/argument-types.md#adding-support-for-other-types) ✔️ |
| \ \ \ Custom Type Descriptors<br/>\ \ \ \ \ \ _Customize parsing and the type name shown in help and_ | [docs](Arguments/argument-types.md#type-descriptors) ✔️ |
| \ \ \ Define allowed values by type<br/>\ \ \ \ \ \ _Allowed values are shown in help and will soon be used for suggestions_ | [docs](Arguments/argument-types.md#type-descriptors) ✔️ |
|   **Argument Values** |  |
| \ \ \ Response Files | [docs](ArgumentValues/response-files.md) ✔️ |
| \ \ \ Piped Input | [docs](ArgumentValues/piped-arguments.md) ✔️ |
| \ \ \ \ \ \ Streaming | [docs](ArgumentValues/piped-arguments.md) ✔️ |
| \ \ \ Prompts | [docs](ArgumentValues/prompting.md) ✔️ |
| \ \ \ \ \ \ Hide passwords | [docs](ArgumentValues/prompting.md#passwords) ✔️ |
| \ \ \ \ \ \ Multi-entry for collections | [docs](ArgumentValues/prompting.md) ✔️ |
| \ \ \ \ \ \ Auto prompt for missing arguments (optional) | [docs](ArgumentValues/prompting.md#prompting-for-missing-arguments) ✔️ |
| \ \ \ Default from EnvVar | [docs](ArgumentValues/default-values-from-config.md) ✔️ |
| \ \ \ Default from AppSetting | [docs](ArgumentValues/default-values-from-config.md) ✔️ |
|   **Validation** |  |
| \ \ \ [FluentValidation](https://github.com/JeremySkinner/FluentValidation) for [argument models](Arguments/argument-models.md) | [docs](Arguments/fluent-validation-for-argument-models.md) ✔️ |
| \ \ \ [Data Annotations](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations) | [#253](https://github.com/bilal-fazlani/commanddotnet/issues/253) ❌ |
|   **Help** |  |
| \ \ \ Autocomplete | [#48](https://github.com/bilal-fazlani/commanddotnet/issues/48) ❌ |
| \ \ \ Typo suggestions | [docs](Help/typo-suggestions.md) ✔️ |
| \ \ \ Auto generated help<br/>\ \ \ \ \ \ _Aliases: -? -h --help_ | [docs](Help/help.md) ✔️ |
| \ \ \ Custom help generators | [docs](Help/help.md#custom-help-provider) ✔️ |  |
|   **Diagnostics** |  |
| \ \ \ App Version<br/>\ \ \ \ \ \ _`-v` or `--version` | [docs](Diagnostics/app-version.md) ✔️ |
| \ \ \ [debug] directive<br/>\ \ \ \ \ \ _step into debugger_ | [docs](Diagnostics/debug-directive.md)  ✔️|
| \ \ \ [parse] directive | [docs](Diagnostics/parse-directive.md) ✔️ |
| \ \ \ \ \ \ show final values | ✔️ |
| \ \ \ \ \ \ show inputs and source<br/>\ \ \ \ \ \ \ \ \ _original source of value, including response file paths_ | ✔️ |
| \ \ \ \ \ \ show defaults and source<br/>\ \ \ \ \ \ \ \ \ _including key if from EnvVar or AppSetting_ | ✔️ |
| \ \ \ Command logging<br/>\ \ \ \ \ \ _show [parse](Diagnostics/parse-directive.md) output and optionally system info and app config_ | [docs](Diagnostics/command-logger.md) ✔️ |
|   **Testing** | [docs](TestTools/overview.md) |
| \ \ \ BDD Framework<br/>\ \ \ \ \ \ _Test an app as if run from the console_ | ✔️ |
| \ \ \ Supports parallel test<br/>\ \ \ \ \ \ _the whole framework avoids static state to support parallel test runs_ | ✔️ |
| \ \ \ TestConsole | ✔️ |  |
| \ \ \ TestDependencyResolver<br/>\ \ \ \ \ \ _new TestDependencyResolver{ dbSvc, httpSvc }_ | ✔️ |
| \ \ \ TempFiles helper<br/>\ \ \ \ \ \ _create and cleanup files used for tests_ | ✔️ |
| \ \ \ Capture State<br/>\ \ \ \ \ \ _Capture state within a run to help test custom middleware components_ | ✔️ |
|   **Dependency Injection** | [docs](OtherFeatures/dependency-injection.md) |
| \ \ \ MicrosoftDependencyInjection | ✔️ |
| \ \ \ Autofac | ✔️ |
| \ \ \ SimpleInjector | ✔️ |
| \ \ \ Test injector | [docs](TestTools/overview.md) ✔️ |
| \ \ \ Custom | ✔️ |
|   **Other** |  |
| \ \ \ Ctrl+C | [docs](OtherFeatures/cancellation.md) ✔️ |
| \ \ \ Name casing<br/>\ \ \ \ \ \ _consistent name casing via [Humanizer](https://github.com/Humanizr/Humanizer)_ | [docs](OtherFeatures/name-casing.md) ✔️ |
|   **Extensibility** |  |
| \ \ \ Custom middleware | [docs](Extensibility/middleware.md) ✔️ |
| \ \ \ Custom directives | [docs](Extensibility/directives.md) ✔️ |
| \ \ \ Custom token transformations | [docs](Extensibility/token-transformations.md) ✔️ |
| \ \ \ Custom parameter resolvers | [docs](Extensibility/parameter-resolvers.md) ✔️ |