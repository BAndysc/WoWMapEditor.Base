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

        public void Bind(Engine engine)
        {
            this.engine = engine;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (engine == null)
                return;

            switch (m.Msg)
            {
                case NativeMethods.WM_LBUTTONUP:
                    engine.inputManager.mouse.MouseUp(Input.MouseButton.Left);
                    return;
                case NativeMethods.WM_LBUTTONDBLCLK:
                case NativeMethods.WM_LBUTTONDOWN:
                    engine.inputManager.mouse.MouseDown(Input.MouseButton.Left);
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
