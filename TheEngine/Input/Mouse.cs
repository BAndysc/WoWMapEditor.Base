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
        
        public Vector2 Position => new Vector2(Cursor.Position.X, Cursor.Position.Y);
        
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
