using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheDX11.Resources;
using TheEngine.Handles;
using TheEngine.Interfaces;
using TheEngine.Structures;
using TheMaths;

namespace TheEngine.Entities
{
    internal class Mesh : IMesh, IDisposable
    {
        private readonly Engine engine;

        internal NativeBuffer<UniversalVertex> VerticesBuffer { get; private set; }
        internal NativeBuffer<int> IndicesBuffer { get; private set; }

        private UniversalVertex[] vertices;
        private int[] indices;

        public int IndexCount => indices.Length;

        public MeshHandle Handle { get; }

        internal Mesh(Engine engine, MeshHandle handle)
        {
            this.engine = engine;
            Handle = handle;
            VerticesBuffer = engine.Device.CreateBuffer<UniversalVertex>(BufferTypeEnum.Vertex, 1);
            IndicesBuffer = engine.Device.CreateBuffer<int>(BufferTypeEnum.Index, 1);
        }

        internal Mesh(Engine engine, MeshHandle handle, UniversalVertex[] vertices, int[] indices)
        {
            this.engine = engine;
            Handle = handle;
            VerticesBuffer = engine.Device.CreateBuffer<UniversalVertex>(BufferTypeEnum.Vertex, vertices);
            IndicesBuffer = engine.Device.CreateBuffer<int>(BufferTypeEnum.Index, indices);
            this.vertices = vertices;
            this.indices = indices;
        }

        public void SetIndices(int[] indices, int submesh)
        {
            this.indices = indices;
        }

        public void SetVertices(Vector3[] vertices)
        {
            this.vertices = vertices.Select(t => new UniversalVertex() { position = new Vector4(t, 1) }).ToArray();
        }

        public void SetColors(Vector4[] colors)
        {
            this.vertices = vertices.Zip(colors, (vert, col) => new UniversalVertex(vert) { color = col }).ToArray();
        }

        public void Rebuild()
        {
            VerticesBuffer.UpdateBuffer(vertices);
            IndicesBuffer.UpdateBuffer(indices);
        }

        public void Dispose()
        {
            VerticesBuffer.Dispose();
            IndicesBuffer.Dispose();

            VerticesBuffer = null;
            IndicesBuffer = null;
        }
    }
}
