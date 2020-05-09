using CommandDotNet.Extensions;

namespace CommandDotNet.TestTools.Scenarios
{
    public class Scenario : IScenario
    {
        /// <summary>The user input</summary>
        public ScenarioWhen When { get; } = new ScenarioWhen();

        /// <summary>The expectations</summary>
        public ScenarioThen Then { get; } = new ScenarioThen();

        public override string ToString()
        {
            return $"{nameof(Scenario)}: args={When.Args ?? When.ArgsArray?.ToCsv(" ")}";
        }
    }
}