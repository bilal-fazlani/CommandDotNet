namespace CommandDotNet.Rendering
{
    public interface IStandardOut
    {
        IConsoleWriter Out { get; }

        bool IsOutputRedirected { get; }
    }
}