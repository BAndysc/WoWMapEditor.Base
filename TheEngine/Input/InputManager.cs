using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheEngine.Input
{
    public class InputManager
    {
        internal Mouse mouse { get; }
        public IMouse Mouse { get; }

        internal Keyboard keyboard { get; }

        public IKeyboard Keyboard { get; }

        internal InputManager(Engine engine)
        {
            Mouse = mouse = new Mouse(engine.WindowHost);
            Keyboard = keyboard = new Keyboard(engine.WindowHost);
        }

        internal void Update()
        {
            mouse.Update();
        }
    }
}
