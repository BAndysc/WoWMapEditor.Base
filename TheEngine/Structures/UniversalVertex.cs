using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheMaths;

namespace TheEngine.Structures
{
    internal struct UniversalVertex
    {
        internal Vector4 position;
        internal Vector4 color;

        internal UniversalVertex(UniversalVertex other)
        {
            position = other.position;
            color = other.color;
        }
    }
}
