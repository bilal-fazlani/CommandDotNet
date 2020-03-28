using System.Threading.Tasks;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class TypeSuggestionsTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TypeSuggestionsTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void TypoSuggestions_In_UseDefaultMiddleware()
        {
            new AppRunner<App>()
                .UseDefaultMiddleware()
                .VerifyScenario(_testOutputHelper,
                    new Scenario
                    {
                        WhenArgs = "User --user",
                        Then =
                        {
                            ExitCode = 1,
                            ResultsContainsTexts =
                            {
                                "'user' is not a option.  See 'dotnet testhost.dll User --help'",
                                @"Similar options are
   --username"
                            }
                        }
                    });
        }

        [Fact]
        public void Given_OptionTypo_ShowsSimilarOptions()
        {
            new AppRunner<App>()
                .UseTypoSuggestions()
                .VerifyScenario(_testOutputHelper,
                    new Scenario
                    {
                        WhenArgs = "User --user",
                        Then =
                        {
                            ExitCode = 1,
                            ResultsContainsTexts =
                            {
                                "'user' is not a option.  See 'dotnet testhost.dll User --help'",
                                @"Similar options are
   --username"
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
                        var option = new Option("usertype", null, a.CommandBuilder.Command, TypeInfo.Single<int>(), ArgumentArity.ExactlyOne)
                        {
                            ShowInHelp = false
                        };
                        a.CommandBuilder.AddArgument(option);
                    })
                .UseTypoSuggestions()
                .VerifyScenario(_testOutputHelper,
                    new Scenario
                    {
                        WhenArgs = "User --user",
                        Then =
                        {
                            ExitCode = 1,
                            ResultsContainsTexts =
                            {
                                "'user' is not a option.  See 'dotnet testhost.dll User --help'",
                                @"Similar options are
   --username"
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
                        var option = new Operand("usertype", a.CommandBuilder.Command, TypeInfo.Single<int>(), ArgumentArity.ExactlyOne);
                        a.CommandBuilder.AddArgument(option);
                    })
                .UseTypoSuggestions()
                .VerifyScenario(_testOutputHelper,
                    new Scenario
                    {
                        WhenArgs = "User --user",
                        Then =
                        {
                            ExitCode = 1,
                            ResultsContainsTexts =
                            {
                                "'user' is not a option.  See 'dotnet testhost.dll User --help'",
                                @"Similar options are
   --username"
                            }
                        }
                    });
        }

        [Fact]
        public void Given_CommandTypo_ShowsSimilarCommands()
        {
            new AppRunner<App>()
                .UseTypoSuggestions()
                .VerifyScenario(_testOutputHelper,
                    new Scenario
                    {
                        WhenArgs = "egister",
                        Then =
                        {
                            ExitCode = 1,
                            ResultsContainsTexts =
                            {
                                "'egister' is not a command.  See 'dotnet testhost.dll  --help'",
                                @"Similar commands are
   Register
   Unregister"
                            }
                        }
                    });
        }

        [Fact]
        public void Given_OptionType_AndManySimilarOptions_LimitResult()
        {
            new AppRunner<App>()
                .UseTypoSuggestions(3)
                .VerifyScenario(_testOutputHelper,
                    new Scenario
                    {
                        WhenArgs = "Similars --opt",
                        Then =
                        {
                            ExitCode = 1,
                            ResultsContainsTexts =
                            {
                                "'opt' is not a option.  See 'dotnet testhost.dll Similars --help'",
                                @"Similar options are
   --opt1
   --opt2
   --opt3"
                            }
                        }
                    });
        }

        [Fact]
        public void Given_OptionType_NoSimilarOptions_ShowsHelp()
        {
            new AppRunner<App>()
                .UseTypoSuggestions()
                .VerifyScenario(_testOutputHelper,
                    new Scenario
                    {
                        WhenArgs = "Similars --lala",
                        Then =
                        {
                            ExitCode = 1,
                            ResultsContainsTexts =
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
                .VerifyScenario(_testOutputHelper,
                    new Scenario
                    {
                        WhenArgs = "--users",
                        Then =
                        {
                            ExitCode = 1,
                            ResultsContainsTexts =
                            {
                                "'users' is not a option.  See 'dotnet testhost.dll  --help'",
                                @"Similar options are
   --username"
                            }
                        }
                    });
        }

        [Fact]
        public void Given_Interceptor_AndCommandTypo_ShowsSimilarCommands_NotOptions()
        {
            new AppRunner<InterceptorApp>()
                .UseTypoSuggestions()
                .VerifyScenario(_testOutputHelper,
                    new Scenario
                    {
                        WhenArgs = "users",
                        Then =
                        {
                            ExitCode = 1,
                            ResultsContainsTexts =
                            {
                                "'users' is not a command.  See 'dotnet testhost.dll  --help'",
                                @"Similar commands are
   ListUsers"
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