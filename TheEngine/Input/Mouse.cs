using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheMaths;

namespace TheEngine.Input
{
    public class Mouse : IMouse
    {
        private volatile bool leftDown;
        private volatile bool rightDown;

        private volatile short mouseWheelDelta;

        public Vector2 Position => new Vector2(Cursor.Position.X, Cursor.Position.Y);

        public short WheelDelta => mouseWheelDelta;

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
            if (button.HasFlag(MouseButton.Left))
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
