# Testing in CommandDotNet

## Using our test harnesses

When implementing new features, there are two patterns for writing feature tests.

* [RunInMem](https://commanddotnet.bilal-fazlani.com/TestTools/Harness/run-in-mem/)
* [BDD Verify](https://commanddotnet.bilal-fazlani.com/TestTools/Harness/bdd/)

Using these patterns ensures we're testing the public API of CommandDotNet which has several benefits

* The full pipeline is tested. Integration between components cannot be accidently skipped.  It is harder to introduce bugs due to not understanding the interactions between subsystems.
* Refactoring internals can be done without changing these tests.
* It's easier to identify breaking changes. If a test needs to change, it indicates a breaking change since either behavior or the API has been modified.

## Defining commands in tests

Both patterns require the test to define the AppRunner<TCommandClass>.

`TCommandClass` should be defined as a private nested class of the test. 

* Making it a nested class makes the relationship unmistakable and easy to follow.
* Making it private prevents polluting the namespace
* Both prevents classes from being used by multiple tests, making it unclear which tests need which configurations in the class.

There is are exceptions to this rule. When implementing a new feature that needs to be validated across datatypes, there exists a shared set of commands in the `FeatureTests.Arguments.Models` namespace. There are two interfaces for arguments defined as parameters and there are a number of IArgumentModels to cover the same use cases for arguments defined as properties.  These exist so use cases are forgotten across different tests and so that if a new use case is added, it is added to all appropriate tests.
