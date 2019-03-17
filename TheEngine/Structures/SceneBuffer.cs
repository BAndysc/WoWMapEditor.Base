using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TheMaths;

namespace TheEngine.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SceneBuffer
    {
        public Matrix ViewMatrix;
        public Matrix ProjectionMatrix;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ObjectBuffer
    {
        public Matrix WorldMatrix;
    }
}
