using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Pujario.Core.Input
{
    
    public readonly struct MouseStateWrap
    {
        public readonly MouseState State;

        public MouseStateWrap(in MouseState mouseState) => State = mouseState;

        public ButtonState this[MouseButtons button] =>
            button switch
            {
                MouseButtons.Back => State.XButton1,
                MouseButtons.Forward => State.XButton2,
                MouseButtons.Left => State.LeftButton,
                MouseButtons.Right => State.RightButton,
                MouseButtons.Middle => State.MiddleButton,
                _ => throw new ArgumentOutOfRangeException(nameof(button), button, null)
            };

        public int this[MouseScrolls scroll] => scroll switch
        {
            MouseScrolls.Horizontal => State.HorizontalScrollWheelValue,
            MouseScrolls.Vertical => State.ScrollWheelValue,
            _ => throw new ArgumentOutOfRangeException(nameof(scroll), scroll, null)
        };

        public Point this[MouseMove move] => move == MouseMove.Any
            ? State.Position
            : throw new ArgumentException("Enum member is invalid");

        public bool IsButtonPressed(MouseButtons button) => this[button] == ButtonState.Pressed;
    }
}