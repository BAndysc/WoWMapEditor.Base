using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheEngine.GUI;
using TheMaths;

namespace TheEngine.Input
{
    public class Mouse : IMouse
    {
        private volatile bool leftDown;
        private volatile bool rightDown;

        private volatile short mouseWheelDelta;
        private readonly IHwndHost window;

        public Vector2 Position => new Vector2(Cursor.Position.X / window.WindowWidth, Cursor.Position.Y / window.WindowHeight);

        public short WheelDelta => mouseWheelDelta;

        internal Mouse(IHwndHost window)
        {
            this.window = window;
        }

        internal void Update()
        {
            mouseWheelDelta = 0;
        }

        internal void MouseWheel(short delta)
        {
            mouseWheelDelta = delta;
        }

        internal void MouseDown(MouseButton button)
        {
            if (button.HasFlag(MouseButton.Left))
                leftDown = true;

            if (button.HasFlag(MouseButton.Right))
                rightDown = true;
        }

        internal void MouseUp(MouseButton button)
        {
            if (button.HasFlag(MouseButton.Left))
                leftDown = false;

            if (button.HasFlag(MouseButton.Right))
                rightDown = false;
        }

        public bool IsMouseDown(MouseButton button)
        {
            if (((int)button & (int)MouseButton.Left) > 0)
                return leftDown;

            return rightDown;
        }
    }

    [Flags]
    public enum MouseButton
    {
        Left = 1,
        Right = 2,
    }
}
