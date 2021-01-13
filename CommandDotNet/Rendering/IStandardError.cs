namespace CommandDotNet.Rendering
{
    public interface IStandardError
    {
        IConsoleWriter Error { get; }

        bool IsErrorRedirected { get; }
    }
}