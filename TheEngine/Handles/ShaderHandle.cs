using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheEngine.Handles
{
    public struct ShaderHandle
    {
        internal int Handle { get; }

        internal ShaderHandle(int id)
        {
            Handle = id;
        }
    }
}
