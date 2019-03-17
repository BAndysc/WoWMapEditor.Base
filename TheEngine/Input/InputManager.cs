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

        internal InputManager(Engine engine)
        {
            Mouse = mouse = new Mouse();
        }
    }
}
