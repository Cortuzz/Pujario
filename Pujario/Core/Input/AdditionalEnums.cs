namespace Pujario.Core.Input
{
    public enum MouseButtons : byte
    {
        Left,
        Middle,
        Right,

        /// <summary>XButton1 or m4</summary>
        Back,

        /// <summary>XButton2 or m5</summary>
        Forward
    }

    public enum MouseMove : byte
    {
        Any
    }

    public enum MouseScrolls : byte
    {
        Vertical,
        Horizontal
    }

    public enum GamePadSticksInput : byte
    {
        Left = 0,
        Right = 1
    }

    public enum GamePadTriggersInput : byte
    {
        Left = 0,
        Right = 1
    }
}