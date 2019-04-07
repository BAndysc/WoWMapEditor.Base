using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheEngine.WinApi;

namespace TheEngine.GUI.WinForms
{
    public class TheEnginePanel : Control, IHwndHost
    {
        private Engine engine;

        public float Aspect => Width * 1.0f / Height;

        public float WindowWidth { get; private set; }

        public float WindowHeight { get; private set; }

        public void Bind(Engine engine)
        {
            this.engine = engine;
            WindowWidth = Width;
            WindowHeight = Height;

            NativeMethods.SetFocus(Handle);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (engine == null)
                return;

            switch (m.Msg)
            {
                case NativeMethods.WM_SIZE:
                    WindowWidth = Width;
                    WindowHeight = Height;
                    return;

                case NativeMethods.WM_KEYDOWN:
                    engine.inputManager.keyboard.KeyDown((System.Windows.Forms.Keys)m.WParam);
                    return;

                case NativeMethods.WM_KEYUP:
                    engine.inputManager.keyboard.KeyUp((System.Windows.Forms.Keys)m.WParam);
                    return;

                case NativeMethods.WM_LBUTTONUP:
                    engine.inputManager.mouse.MouseUp(Input.MouseButton.Left);
                    return;
                case NativeMethods.WM_LBUTTONDBLCLK:
                case NativeMethods.WM_LBUTTONDOWN:
                    engine.inputManager.mouse.MouseDown(Input.MouseButton.Left);
                    NativeMethods.SetFocus(Handle);
                    return;
                case NativeMethods.WM_RBUTTONUP:
                    engine.inputManager.mouse.MouseUp(Input.MouseButton.Right);
                    return;
                case NativeMethods.WM_RBUTTONDBLCLK:
                case NativeMethods.WM_RBUTTONDOWN:
                    engine.inputManager.mouse.MouseDown(Input.MouseButton.Right);
                    return;

                case NativeMethods.WM_MOUSEWHEEL:
                    engine.inputManager.mouse.MouseWheel((short)((long)m.WParam >> 16));
                    return;
            }
        }
    }
}
