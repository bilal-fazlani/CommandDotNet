namespace CommandDotNet.Rendering
{
    public interface IConsoleCursor
    {
        int CursorSize { get; set; }
        bool CursorVisible { get; set; }
        int CursorLeft { get; set; }
        int CursorTop { get; set; }
        void SetCursorPosition(int left, int top);
    }
}