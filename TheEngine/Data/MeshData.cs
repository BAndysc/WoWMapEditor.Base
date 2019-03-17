using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheMaths;

namespace TheEngine.Data
{
    public class MeshData
    {
        public Vector3[] Vertices { get; }
        public Vector3[] Normals { get; }
        public Vector2[] UV { get; }
        public int[] Indices { get; }

        public MeshData(Vector3[] vertices, Vector3[] normals, Vector2[] uvs, int[] indices)
        {
            Vertices = vertices;
            Normals = normals;
            UV = uvs;
            Indices = indices;
        }
    }
}
