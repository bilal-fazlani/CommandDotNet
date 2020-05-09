using System.Threading.Tasks;
using CommandDotNet.TestTools.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Suggestions
{
    public class TypoSuggestionsTests
    {
        public TypoSuggestionsTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        /* Test Matrix
            Typos:          OptionType vs ValueTypo (Command or AllowedValues)
            Option sources: Command method, Interceptors, Middleware (Hidden & Visible)
            AllowedValue:   Options & Operands
            Argument Arity: Flags, Single, List
            Other:          Default Command method
        */

        // TODO: Tests
        // - default method
        // - option allowed values
        // - argument allowed values
        // - argument list allowed values

        [Fact]
        public void TypoSuggestions_IsIncludedWith_UseDefaultMiddleware()
        {
            new AppRunner<App>()
                .UseDefaultMiddleware()
                .Verify(new Scenario
                {
                    When = {Args = "User --user"},
                    Then =
                    {
                        ExitCode = 1,
                        AssertContext = ctx => ctx.AppConfig.MiddlewarePipeline.Should()
                            .Contain(p => p.Method.Name == "TypoSuggest")
                    }
                });
        }

        [Fact]
        public void TypoSuggestions_NotIncludedWith_CoreMiddleware()
        {
            new AppRunner<App>()
                .Verify(new Scenario
                {
                    When = {Args = "User --user"},
                    Then =
                    {
                        ExitCode = 1,
                        AssertContext = ctx => ctx.AppConfig.MiddlewarePipeline.Should()
                            .NotContain(p => p.Method.Name == "TypoSuggest")
                    }
                });
        }

        [Fact]
        public void Given_OptionTypo_ShowsSimilarOptions()
        {
            new AppRunner<App>()
                .UseTypoSuggestions()
                .Verify(new Scenario
                {
                    When = {Args = "User --user"},
                    Then =
                    {
                        ExitCode = 1,
                        OutputContainsTexts =
                        {
                            @"'user' is not a valid option

Did you mean ...
   --username

See 'dotnet testhost.dll User --help'
"
                        }
                    }
                });
        }

        [Fact]
        public void Given_OptionTypo_ExcludesHiddenOptions()
        {
            new AppRunner<App>()
                .Configure(c => 
                    c.BuildEvents.OnCommandCreated += a =>
                    {
                        var option = new Option("usertype", null, TypeInfo.Single<int>(), ArgumentArity.ExactlyOne)
                        {
                            Hidden = true
                        };
                        a.CommandBuilder.AddArgument(option);
                    })
                .UseTypoSuggestions()
                .Verify(
                    new Scenario
                    {
                        When = {Args = "User --user"},
                        Then =
                        {
                            ExitCode = 1,
                            OutputContainsTexts =
                            {
                                @"'user' is not a valid option

Did you mean ...
   --username

See 'dotnet testhost.dll User --help'
"
                            }
                        }
                    });
        }

        [Fact]
        public void Given_OptionTypo_ExcludesOperands()
        {
            new AppRunner<App>()
                .Configure(c =>
                    c.BuildEvents.OnCommandCreated += a =>
                    {
                        var option = new Operand("usertype", TypeInfo.Single<int>(), ArgumentArity.ExactlyOne);
                        a.CommandBuilder.AddArgument(option);
                    })
                .UseTypoSuggestions()
                .Verify(
                    new Scenario
                    {
                        When = {Args = "User --user"},
                        Then =
                        {
                            ExitCode = 1,
                            OutputContainsTexts =
                            {
                                @"'user' is not a valid option

Did you mean ...
   --username

See 'dotnet testhost.dll User --help'
"
                            }
                        }
                    });
        }

        [Fact]
        public void Given_ValueTypo_ShowsSimilarCommands()
        {
            new AppRunner<App>()
                .UseTypoSuggestions()
                .Verify(
                    new Scenario
                    {
                        When = {Args = "egister"},
                        Then =
                        {
                            ExitCode = 1,
                            OutputContainsTexts =
                            {
                                @"'egister' is not a valid subcommand

Did you mean ...
   Register
   Unregister
   User

See 'dotnet testhost.dll  --help'
"
                            }
                        }
                    });
        }

        [Fact]
        public void Given_ValueType_AndManySimilarOptions_LimitResult()
        {
            new AppRunner<App>()
                .UseTypoSuggestions(3)
                .Verify(
                    new Scenario
                    {
                        When = {Args = "Similars --opt"},
                        Then =
                        {
                            ExitCode = 1,
                            OutputContainsTexts =
                            {
                                @"'opt' is not a valid option

Did you mean ...
   --opt1
   --opt2
   --opt3

See 'dotnet testhost.dll Similars --help'
"
                            }
                        }
                    });
        }

        [Fact]
        public void Given_ValueType_NoSimilarOptions_ShowsHelp()
        {
            new AppRunner<App>()
                .UseTypoSuggestions()
                .Verify(
                    new Scenario
                    {
                        When = {Args = "Similars --lala"},
                        Then =
                        {
                            ExitCode = 1,
                            OutputContainsTexts =
                            {
                                "Unrecognized option '--lala'"
                            }
                        }
                    });
        }

        [Fact]
        public void Given_Interceptor_AndOptionTypo_ShowsSimilarOptions_NotCommands()
        {
            new AppRunner<InterceptorApp>()
                .UseTypoSuggestions()
                .Verify(
                    new Scenario
                    {
                        When = {Args = "--users"},
                        Then =
                        {
                            ExitCode = 1,
                            OutputContainsTexts =
                            {
                                @"'users' is not a valid option

Did you mean ...
   --username

See 'dotnet testhost.dll  --help'
"
                            }
                        }
                    });
        }

        [Fact]
        public void Given_Interceptor_AndValueTypo_ShowsSimilarCommands_NotOptions()
        {
            new AppRunner<InterceptorApp>()
                .UseTypoSuggestions()
                .Verify(
                    new Scenario
                    {
                        When = {Args = "users"},
                        Then =
                        {
                            ExitCode = 1,
                            OutputContainsTexts =
                            {
                                @"'users' is not a valid subcommand

Did you mean ...
   ListUsers

See 'dotnet testhost.dll  --help'"
                            }
                        }
                    });
        }

        public class App
        {
            public void User([Option] string username, [Option] string firstname, [Option] string lastname)
            {
            }

            public void Register() { }

            public void Unregister() { }

            public void Similars([Option] string opt1, [Option] string opt2, [Option] string opt3, [Option] string opt4, [Option] string opt5) { }
        }

        public class InterceptorApp
        {
            public Task<int> Authenticate(InterceptorExecutionDelegate next, [Option] string username, [Option] string password)
            {
                return next();
            }

            public void ListUsers() { }
        }
    }
}
