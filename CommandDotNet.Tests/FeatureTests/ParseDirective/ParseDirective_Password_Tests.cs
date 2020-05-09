using System;
using System.Collections.Specialized;
using CommandDotNet.TestTools;
using CommandDotNet.TestTools.Prompts;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ParseDirective
{
    public class ParseDirective_Password_Tests : IDisposable
    {
        private readonly TempFiles _tempFiles;

        public ParseDirective_Password_Tests(ITestOutputHelper output)
        {
            Ambient.Output = output;
            _tempFiles = new TempFiles(output.WriteLine);
        }

        public void Dispose()
        {
            _tempFiles.Dispose();
        }

        [Fact]
        public void Given_InputFromShell_OnlyExposedInTokenTransformations()
        {
            new AppRunner<App>()
                .UseParseDirective()
                .Verify(new Scenario
                {
                    When = {Args = "[parse:t] Secure -u me -p super-secret"},
                    Then =
                    {
                        Output = @"command: Secure

options:

  username <Text>
    value: me
    inputs: me (from: -u me)
    default:

  password <Text>
    value: *****
    inputs: ***** (from: -p *****)
    default: *****

token transformations:

>>> from shell
  Directive: [parse:t]
  Value    : Secure
  Option   : -u
  Value    : me
  Option   : -p
  Value    : super-secret
>>> after: expand-clubbed-flags (no changes)
>>> after: split-option-assignments (no changes)
"
                    }
                });
        }

        [Fact]
        public void Given_InputFromResponseFile_OnlyExposedInTokenTransformations()
        {
            var tempFile = _tempFiles.CreateTempFile("-u me -p super-secret");
            new AppRunner<App>()
                .UseResponseFiles()
                .UseParseDirective()
                .Verify(new Scenario
                {
                    When = {Args = $"[parse:t] Secure @{tempFile}"},
                    Then =
                    {
                        Output = $@"command: Secure

options:

  username <Text>
    value: me
    inputs: me (from: @{tempFile} -> -u me)
    default:

  password <Text>
    value: *****
    inputs: ***** (from: @{tempFile} -> -p *****)
    default: *****

token transformations:

>>> from shell
  Directive: [parse:t]
  Value    : Secure
  Value    : @{tempFile}
>>> after: expand-response-files
  Directive: [parse:t]
  Value    : Secure
  Option   : -u
  Value    : me
  Option   : -p
  Value    : super-secret
>>> after: expand-clubbed-flags (no changes)
>>> after: split-option-assignments (no changes)
"
                    }
                });
        }

        [Fact]
        public void Given_InputFromPrompt_OnlyExposedInTokenTransformations()
        {
            new AppRunner<App>()
                .UsePrompting()
                .UseParseDirective()
                .Verify(new Scenario
                {
                    When =
                    {
                        Args = "[parse:t] PromptSecure",
                        OnPrompt = Respond.WithText("super-secret")
                    },
                    Then =
                    {
                        Output = @"password (Text): 
command: PromptSecure

arguments:

  password <Text>
    value: *****
    inputs: [prompt] *****
    default:

token transformations:

>>> from shell
  Directive: [parse:t]
  Value    : PromptSecure
>>> after: expand-clubbed-flags (no changes)
>>> after: split-option-assignments (no changes)
"
                    }
                });
        }

        [Fact]
        public void Given_DefaultValueFromCustom_OnlyExposedInTokenTransformations()
        {
            var appSettings = new NameValueCollection
            {
                {"--password", "super-secret"}
            };

            new AppRunner<App>()
                .UseDefaultsFromAppSetting(appSettings, includeNamingConventions: true)
                .UseParseDirective()
                .Verify(new Scenario
                {
                    When = {Args = "[parse:t] Secure -u me"},
                    Then =
                    {
                        Output = @"command: Secure

options:

  username <Text>
    value: me
    inputs: me (from: -u me)
    default:

  password <Text>
    value: *****
    inputs:
    default: source=AppSetting key=--password: *****

token transformations:

>>> from shell
  Directive: [parse:t]
  Value    : Secure
  Option   : -u
  Value    : me
>>> after: expand-clubbed-flags (no changes)
>>> after: split-option-assignments (no changes)
"
                    }
                });
        }


        class App
        {
            public void Secure(Args args)
            {
            }

            public void PromptSecure(Password password)
            {
            }
        }

        class Args : IArgumentModel
        {
            [Option(ShortName = "u", LongName = "username")]
            public string? Username { get; set; }

            [Option(ShortName = "p", LongName = "password")]
            public Password Password { get; set; } = new Password("default-secret");

        }
    }
}