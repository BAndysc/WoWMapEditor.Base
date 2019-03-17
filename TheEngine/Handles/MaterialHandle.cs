using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheEngine.Handles
{
    public struct MaterialHandle
    {
        internal int Handle { get; }

        internal MaterialHandle(int id)
        {
            Handle = id;
        }
    }
}
