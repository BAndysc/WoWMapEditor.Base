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
        public float Time;

        public float Align1;
        public float Align2;
        public float Align3;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PixelShaderSceneBuffer
    {
        public Vector4 LightDirection;
        public Vector4 LightColor;
        public float Time;

        public float Align1;
        public float Align2;
        public float Align3;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ObjectBuffer
    {
        public Matrix WorldMatrix;
    }
}
