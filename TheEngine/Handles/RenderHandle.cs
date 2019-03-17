using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheEngine.Handles
{
    public struct RenderHandle
    {
        internal int Handle { get; }

        internal RenderHandle(int id)
        {
            Handle = id;
        }
    }
}
