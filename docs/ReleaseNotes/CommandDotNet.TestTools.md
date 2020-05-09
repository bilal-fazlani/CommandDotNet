# CommandDotNet.TestTools

## 3.0.0 - prerelease

### Nullable Reference Types

The library has been updated to support Nullable Reference Types

### Obsoletes

Remove obsoleted TestCaptures class

## 2.1.1

### Getting invocations in tests

Adding CommandContext extensions `GetCommandInvocation` and `GetInterceptorInvocation<TInterceptorClass>` that give access to invocation of the command and interceptor methods.

With these extensions, tests for middleware can verify the values passed to the methods instead of using TestCaptures.

### Goodbye TestCaptures

TestCaptures and associated methods have been marked obsolete. They'll be removed in the next major version. This was added before v3 as a way to cpature the parameter values passed into methods.  With v3 complete, it's easy to access the parameter values via `CommandContext.InvocationPipeline`. Scenarios can now `AssertContext` which removes the need for TestCaptures. This also removes the need for the custom ShouldBeEquivalentTo.

## 2.1.0

Remove FluentValidator requirement. Implemented simple ShouldBeEquivalentTo that handles IEnumerables, primitives and deep recursion of public properties of objects. This is only used with TestCaptures.

Obsoleted the Output___ members of AppRunnerResult. They'll be remove in the next release.

## 2.0.1

More assert options for BDD Scenario `Then = { AssertOutput..., AssertContext... }`. See [BDD Verify > Then](../TestTools/Harness/bdd.md#then) for details.

## 2.0.0

This release focuses on usability, readability and discoverability.

The tooling was initially designed for CommandDotNet with an aspiration for general use. The tooling did work for other projects but required those projects to write shims to make the tooling more useful.

The primary test extensions: `RunInMem` and `Verify` are now easier to use, provide a more consistent experience and no longer duplicate output in some scenarios.

### PromptResponder

* Extract IAnswer and split answers into separate types: TextAnswer, ListAnswer, Answer, FailAnswer
* replace FailOnPromptResponder with FailAnswer

### TestConfig

TestConfig provides settings primarily to determine what is logged during a test run, on success and on error.

See [TestConfig docs](../TestTools/Harness/test-config.md) for more details.

### Scenario.Then Output vs Result

Clean up naming confusion in Scenarios: Ouputs vs Result. 

TestOutputs represented objects (usually parameters) that were captured in a command method. 
They help test middleware and framework components. The `Then.Outputs` used a similar name but it 
was more easily interpretted as `Console.Out` which was actually found in `Then.Result`.

Renaming TestOutputs to TestCaptures made this simpler.

To update, use find/replace

* TestOutputs > TestCaptures
* Outputs = > Captures =
* Result = > Output =
* ResultsContainsTexts > OutputContainsTexts
* ResultsNotContainsTexts > OutputNotContainsTexts

### Scenerio.Given moved to Scenario.When

The properties in Given were all user inputs that would occur after the arguments are provided so it makes more sense to include them in When.

Given is the context of the fully configured AppRunner.

This better aligns with the intent of the Given-When-Then style.

### Scenario.WhenArgs & WhenArgsArray moved to Scenario.When

`Scenario.WhenArgs = ...` -> `Scenario.When = {Args = ...}`

### AppRunnerResult.Console___ properties moved to TestConsole

* AppRunnerResult.ConsoleOutAndError -> AppRunnerResult.Console.AllText()
* AppRunnerResult.ConsoleOut -> AppRunnerResult.Console.OutText()
* AppRunnerResult.ConsoleError -> AppRunnerResult.Console.ErrorText()

### AppRunnerResult.ConsoleOut -> Console.AllText()

Renamed AppRunnerResult.ConsoleOutAndError to ConsoleAll

### AppRunnerResult Output assertions

No longer trim trailing whitespace and replace line endings. 

This was originally necessary to support the HelpTextProvider and inconsistencies with NewLine's used.  Those have all been fixed so we no longer need to do this.

### Scenario.Given.AppSettings

Removed ScenarioGiven.AppSettings. It did not work outside of old CommandDotNet test infrastructure. Apologies for any confusion it may have caused.

### VerifyScenario -> Verify

renamed VerifyScenario extension method to Verify. It reads better as `appRunner.Verify(new Scenario{...})`

### ILogger -> Action<string>
replaced ILogger with Action<string>. ILogger never ended up provided more value and was a needless abstraction.

## 1.1.1

RunInMem will print the stacktrace for of a caught exception before returning the error response, otherwise the stacktrace is lost.

## 1.1.0

Test prompting no longer prints an extraneous `$"IConsole.ReadLine > {input}"` as it's not consistent with console output. This is logged instead.

## 1.0.2

Ensure log provider is always configured for appRunner RunInMem and VerifyScenario extensions

## 1.0.1

make TestToolsLogProvider publicly available

VerifyScenario now captures exceptions that escape AppRunner.Run, putting them into the result to verify as console output.  
This better mimics the behavior of the shell which will return an error code of 1 and print the exception

VerifyScenario will also logging more context on error, including most of AppConfig

improve stack traces with AppOutputBase and PathMap project settings
```
<AppOutputBase>$(MSBuildProjectDirectory)\</AppOutputBase>
<PathMap>$(AppOutputBase)=CommandDotNet/</PathMap>
```