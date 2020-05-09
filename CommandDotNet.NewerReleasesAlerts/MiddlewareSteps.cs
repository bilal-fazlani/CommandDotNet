using CommandDotNet.Execution;

namespace CommandDotNet.NewerReleasesAlerts
{
    public static class MiddlewareSteps
    {
        public static MiddlewareStep NewerReleaseAlerts { get; set; } = new MiddlewareStep(MiddlewareStages.PostParseInputPreBindValues);
    }
}