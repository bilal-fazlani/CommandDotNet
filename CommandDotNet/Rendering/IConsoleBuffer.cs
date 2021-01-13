namespace CommandDotNet.Rendering
{
    public interface IConsoleBuffer
    {
        int BufferWidth { get; set; }
        int BufferHeight { get; set; }
        void SetBufferSize(int width, int height);
        void Clear();
    }
}