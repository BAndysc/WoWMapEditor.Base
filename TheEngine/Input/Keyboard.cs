using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheEngine.GUI;

namespace TheEngine.Input
{
    internal class Keyboard : IKeyboard
    {
        private IHwndHost windowHost;

        private volatile bool[] downKeys = new bool[255];

        public Keyboard(IHwndHost windowHost)
        {
            this.windowHost = windowHost;
        }

        internal void KeyDown(Keys key)
        {
            if (key >= 0 && (int)key <= 255)
                downKeys[(int)key] = true;
        }

        internal void KeyUp(Keys key)
        {
            if (key >= 0 && (int)key <= 255)
                downKeys[(int)key] = false;
        }
        public bool IsDown(System.Windows.Forms.Keys keys)
        {
            return downKeys[(int)keys];
        }
    }
}
