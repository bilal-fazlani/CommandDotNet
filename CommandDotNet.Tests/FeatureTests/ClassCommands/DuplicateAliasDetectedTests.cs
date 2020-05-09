using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.ClassCommands
{
    public class DuplicateAliasDetectedTests
    {
        public DuplicateAliasDetectedTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void ShouldDetectDuplicateCommands()
        {
            Assert.Throws<InvalidConfigurationException>(() =>
                    new AppRunner<DuplicateCommandApp>()
                        .RunInMem("-h"))
                .Message.Should().Be("Duplicate alias 'lala' added to command 'DuplicateCommandApp'. Duplicates: " +
                                     "'Command:lala(source:CommandDotNet.Tests.FeatureTests.ClassCommands.DuplicateAliasDetectedTests+DuplicateCommandApp.Do2)' & " +
                                     "'Command:lala(source:CommandDotNet.Tests.FeatureTests.ClassCommands.DuplicateAliasDetectedTests+DuplicateCommandApp.Do1)'");
        }

        [Fact]
        public void ShouldDetectDuplicateArgs()
        {
            Assert.Throws<InvalidConfigurationException>(() =>
                    new AppRunner<DuplicateArgumentApp>()
                        .RunInMem("-h"))
                .Message.Should().Be("Duplicate alias 'lala' added to command 'Do'. Duplicates: " +
                                     "'Option:lala(source:CommandDotNet.Tests.FeatureTests.ClassCommands.DuplicateAliasDetectedTests+DuplicateArgumentApp.Do.option)' & " +
                                     "'Operand:lala(source:CommandDotNet.Tests.FeatureTests.ClassCommands.DuplicateAliasDetectedTests+DuplicateArgumentApp.Do.operand)'");
        }

        [Fact]
        public void ShouldDetectDuplicateInterceptorOptions()
        {
            Assert.Throws<InvalidConfigurationException>(() =>
                    new AppRunner<DuplicateInterceptorOptionApp>()
                        .RunInMem("-h"))
                .Message.Should().Be("Duplicate alias 'lala' added to command 'DuplicateInterceptorOptionApp'. Duplicates: " +
                                     "'Command:lala(source:CommandDotNet.Tests.FeatureTests.ClassCommands.DuplicateAliasDetectedTests+DuplicateInterceptorOptionApp.Do)' & " +
                                     "'Option:lala(source:CommandDotNet.Tests.FeatureTests.ClassCommands.DuplicateAliasDetectedTests+DuplicateInterceptorOptionApp.Intercept.lala)'");
        }

        [Fact]
        public void ShouldDetectDuplicateInheritedOptions()
        {
            Assert.Throws<InvalidConfigurationException>(() =>
                    new AppRunner<DuplicateInheritedOptionApp>()
                        .RunInMem("SubApp SubDo -h"))
                .Message.Should().Be("Duplicate alias 'lala' added to command 'SubDo'. Duplicates: " +
                                     "'Option:lala(source:CommandDotNet.Tests.FeatureTests.ClassCommands.DuplicateAliasDetectedTests+DuplicateInheritedOptionApp.Intercept.lala)' & " +
                                     "'Operand:lala(source:CommandDotNet.Tests.FeatureTests.ClassCommands.DuplicateAliasDetectedTests+DuplicateInheritedOptionApp+SubApp.SubDo.lala)'");
        }

        [Fact]
        public void InteceptorOptionsShouldNotCollideWithChildCommandOptions()
        {
            new AppRunner<NonDuplicateInheritedOptionApp>()
                .RunInMem("SubApp SubDo -h")
                .ExitCode.Should().Be(0);
        }

        class DuplicateCommandApp
        {
            [Command(Name = "lala")]
            public void Do1(string text) { }
            [Command(Name = "lala")]
            public void Do2(int number) { }
        }

        class DuplicateArgumentApp
        {
            public void Do(
                [Operand(Name = "lala")] string operand,
                [Option(LongName = "lala")] string option)
            { }
        }

        class DuplicateInterceptorOptionApp
        {
            public Task<int> Intercept(InterceptorExecutionDelegate next, string lala)
            {
                return next();
            }

            [Command(Name = "lala")]
            public void Do() { }
        }

        class DuplicateInheritedOptionApp
        {
            public Task<int> Intercept(InterceptorExecutionDelegate next, [Option(AssignToExecutableSubcommands = true)] string lala)
            {
                return next();
            }

            public void Do() { }

            [SubCommand]
            public class SubApp
            {
                public void SubDo(string lala) { }
            }
        }

        class NonDuplicateInheritedOptionApp
        {
            public Task<int> Intercept(InterceptorExecutionDelegate next, [Option] string lala)
            {
                return next();
            }

            public void Do() { }

            [SubCommand]
            public class SubApp
            {
                public void SubDo(string lala) { }
            }
        }
    }
}