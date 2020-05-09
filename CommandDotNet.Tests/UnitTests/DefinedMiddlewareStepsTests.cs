using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.UnitTests
{
    public class DefinedMiddlewareStepsTests
    {
        private readonly ITestOutputHelper _output;

        public DefinedMiddlewareStepsTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(Skip = "for reporting only")]
        //[Fact]
        public void List()
        {
            var definedSteps = GetDefinedMiddlewareSteps()
                .OrderBy(s => s.step.Stage)
                .ThenBy(s => s.step.OrderWithinStage)
                .ToList();
            PrintSteps(definedSteps);
        }

        [Fact]
        public void MiddlewareSteps_Should_HaveUniqueOrder()
        {
            var definedSteps = GetDefinedMiddlewareSteps()
                .OrderBy(s => s.step.Stage)
                .ThenBy(s => s.step.OrderWithinStage)
                .ToList();

            var duplicatedSteps = new HashSet<Step>();

            var previousStep = definedSteps.First();

            foreach (var step in definedSteps.Skip(1))
            {
                if (step.step == previousStep.step)
                {
                    duplicatedSteps.Add(previousStep);
                    duplicatedSteps.Add(step);
                }
                else
                {
                    previousStep = step;
                }
            }

            if (duplicatedSteps.Any())
            {
                _output.WriteLine("Duplicated steps");
                PrintSteps(duplicatedSteps);

                _output.WriteLine("");
                _output.WriteLine("All steps");
                PrintSteps(definedSteps);

                duplicatedSteps.Should().BeEmpty();
            }
        }

        private void PrintSteps(IEnumerable<Step> steps)
        {
            var maxNameLength = steps.Max(s => s.name.Length);
            var maxStageNameLength = steps.Max(s => s.step.Stage.ToString().Length);

            var message = steps
                .Select(s =>
                    $"  {s.name.PadRight(maxNameLength)} > {s.step.Stage.ToString().PadRight(maxStageNameLength)}:{s.step.OrderWithinStage}")
                .ToCsv(Environment.NewLine);

            _output.WriteLine(message);
        }

        private static IEnumerable<Step> GetDefinedMiddlewareSteps()
        {
            foreach (var nestedClass in GetNestedClassesIncludingSelf(typeof(MiddlewareSteps)))
            {
                var steps = nestedClass.type.GetProperties(BindingFlags.Public | BindingFlags.Static)
                    .Where(p => p.PropertyType == typeof(MiddlewareStep))
                    .Select(p => new Step($"{nestedClass.name}.{p.Name}", (MiddlewareStep)p.GetValue(null)!));

                foreach (var step in steps)
                {
                    yield return step;
                }
            }
        }

        private static IEnumerable<(string name, Type type)> GetNestedClassesIncludingSelf(Type type)
        {
            yield return (type.Name, type);
            foreach (var nestedClass in GetNestedClasses(type, type.Name))
            {
                yield return nestedClass;
            }
        }

        private static IEnumerable<(string name, Type type)> GetNestedClasses(Type type, string typePath)
        {
            foreach (var nestedType in type.GetNestedTypes(BindingFlags.Public | BindingFlags.Static))
            {
                var newTypePath = $"{typePath}.{nestedType.Name}";
                yield return (newTypePath, nestedType);
                foreach (var nestedClass in GetNestedClasses(nestedType, newTypePath))
                {
                    yield return nestedClass;
                }
            }
        }

        private class Step
        {
            public string name { get; }
            public MiddlewareStep step { get; }

            public Step(string name, MiddlewareStep step)
            {
                this.name = name;
                this.step = step;
            }

            public override string ToString()
            {
                return $"Step: {name} > {step.Stage}:{step.OrderWithinStage}";
            }
        }
    }
}