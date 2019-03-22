using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheEngine.Handles
{
    public struct TextureHandle
    {
        internal int Handle { get; }

        internal TextureHandle(int id)
        {
            Handle = id;
        }
    }
}
