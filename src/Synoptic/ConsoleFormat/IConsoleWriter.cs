namespace Synoptic.ConsoleFormat
{
    public interface IConsoleWriter
    {
        void WriteLine();
        void Write(string format, params string[] args);
        void SetStyle(ConsoleStyle style);
        void ResetStyle();
        int GetWidth();
        int GetCursorColumnPosition();
    }
}