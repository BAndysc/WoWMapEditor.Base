using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheEngine.Handles;
using TheMaths;

namespace TheEngine.Interfaces
{
    public interface IMesh
    {
        void SetVertices(Vector3[] vertices);
        void SetIndices(int[] indices, int submesh);
        void SetUV(Vector2[] uvs, int slot);
        void SetColors(Vector4[] colors);
        void Rebuild();

        int IndexCount { get; }

        MeshHandle Handle { get; }
    }
}
