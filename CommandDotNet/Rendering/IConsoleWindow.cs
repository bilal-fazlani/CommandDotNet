namespace CommandDotNet.Rendering
{
    public interface IConsoleWindow
    {
        int WindowLeft { get; set; }
        int WindowTop { get; set; }
        int WindowWidth { get; set; }
        int WindowHeight { get; set; }
        void SetWindowPosition(int left, int top);
        void SetWindowSize(int width, int height);
    }
}