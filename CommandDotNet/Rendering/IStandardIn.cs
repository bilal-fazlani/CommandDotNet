namespace CommandDotNet.Rendering
{
    public interface IStandardIn
    {
        IConsoleReader In { get; }

        bool IsInputRedirected { get; }
    }
}