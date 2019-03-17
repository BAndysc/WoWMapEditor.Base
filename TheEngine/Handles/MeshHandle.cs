using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheEngine.Handles
{
    public struct MeshHandle
    {
        internal int Handle { get; }

        internal MeshHandle(int id)
        {
            Handle = id;
        }
    }
}
