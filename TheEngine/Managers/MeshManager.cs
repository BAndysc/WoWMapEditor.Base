using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheEngine.Data;
using TheEngine.Entities;
using TheEngine.Handles;
using TheEngine.Interfaces;
using TheEngine.Structures;
using TheMaths;

namespace TheEngine.Managers
{
    public class MeshManager : IMeshManager, IDisposable
    {
        private readonly Engine engine;

        private List<Mesh> meshes;

        internal MeshManager(Engine engine)
        {
            meshes = new List<Mesh>();
            this.engine = engine;
        }

        // @todo: not thread safe!
        public IMesh CreateMesh(Vector3[] vertices, int[] indices)
        {
            var handle = new MeshHandle(meshes.Count);

            var mesh = new Mesh(engine, handle);
            mesh.SetVertices(vertices);
            mesh.SetIndices(indices, 0);
            mesh.Rebuild();

            meshes.Add(mesh);

            return mesh;
        }

        // @todo: not thread safe!
        public IMesh CreateMesh(MeshData meshData)
        {
            var handle = new MeshHandle(meshes.Count);

            UniversalVertex[] vertices = new UniversalVertex[meshData.Vertices.Length];
            for (int i = 0; i < vertices.Length; ++i)
            {
                vertices[i] = new UniversalVertex()
                {
                    position = new Vector4(meshData.Vertices[i], 1),
                    normal = new Vector4(meshData.Normals[i], 0)
                };
            }
            
            var mesh = new Mesh(engine, handle, vertices, meshData.Indices);
            meshes.Add(mesh);

            return mesh;
        }

        public void Dispose()
        {
            foreach (var mesh in meshes)
            {
                mesh.Dispose();
            }

            meshes.Clear();
        }

        internal Mesh GetMeshByHandle(MeshHandle mesh)
        {
            return meshes[mesh.Handle];
        }
    }
}
