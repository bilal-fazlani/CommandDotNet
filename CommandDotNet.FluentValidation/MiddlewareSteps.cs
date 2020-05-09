using CommandDotNet.Execution;

namespace CommandDotNet.FluentValidation
{
    public static class MiddlewareSteps
    {
        public static MiddlewareStep FluentValidation { get; set; } = new MiddlewareStep(MiddlewareStages.PostBindValuesPreInvoke);
    }
}