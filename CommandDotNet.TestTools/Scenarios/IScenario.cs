namespace CommandDotNet.TestTools.Scenarios
{
    /// <summary>A command line scenario</summary>
    public interface IScenario
    {
        /// <summary>The starting context</summary>
        ScenarioWhen When { get; }

        /// <summary>The expectations</summary>
        ScenarioThen Then { get; }
    }
}