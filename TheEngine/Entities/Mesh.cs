using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheDX11.Resources;
using TheEngine.Interfaces;
using TheEngine.Structures;
using TheMaths;

namespace TheEngine.Entities
{
    internal class Mesh : IMesh, IDisposable
    {
        private readonly Engine engine;

        private NativeBuffer<UniversalVertex> verticesBuffer;
        private NativeBuffer<int> indicesBuffer;

        private UniversalVertex[] vertices;
        private int[] indices;

        internal Mesh(Engine engine)
        {
            this.engine = engine;
            verticesBuffer = engine.Device.CreateBuffer<UniversalVertex>(BufferTypeEnum.Vertex, 1);
            indicesBuffer = engine.Device.CreateBuffer<int>(BufferTypeEnum.Index, 1);
        }

        public void SetIndices(int[] indices, int submesh)
        {
            this.indices = indices;
        }

        public void SetVertices(Vector3[] vertices)
        {
            this.vertices = vertices.Select(t => new UniversalVertex() { position = t }).ToArray();
        }

        public void Rebuild()
        {
            verticesBuffer.UpdateBuffer(vertices);
            indicesBuffer.UpdateBuffer(indices);
        }

        public void Dispose()
        {
            verticesBuffer.Dispose();
            indicesBuffer.Dispose();

            verticesBuffer = null;
            indicesBuffer = null;
        }
    }
}
