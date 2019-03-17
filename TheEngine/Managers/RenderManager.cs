using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheDX11.Resources;
using TheEngine.Config;
using TheEngine.Entities;
using TheEngine.Handles;
using TheEngine.Interfaces;
using TheEngine.Structures;
using TheMaths;

namespace TheEngine.Managers
{
    public class RenderManager : IRenderManager, IDisposable
    {
        private readonly Engine engine;

        private NativeBuffer<SceneBuffer> sceneBuffer;
        private NativeBuffer<ObjectBuffer> objectBuffer;

        private SceneBuffer sceneData;

        private ICameraManager cameraManager;

        private List<RendererEntry> renderers = new List<RendererEntry>();

        internal RenderManager(Engine engine)
        {
            this.engine = engine;

            cameraManager = engine.CameraManager;

            sceneBuffer = engine.Device.CreateBuffer<SceneBuffer>(BufferTypeEnum.ConstVertex, 1);
            objectBuffer = engine.Device.CreateBuffer<ObjectBuffer>(BufferTypeEnum.ConstVertex, 1);

            sceneData = new SceneBuffer();
        }
        
        public void Dispose()
        {
            objectBuffer.Dispose();
            sceneBuffer.Dispose();
        }

        internal void Render()
        {
            UpdateSceneBuffer();

            engine.Device.RenerClearBuffer();

            sceneBuffer.UpdateBuffer(sceneData);
            sceneBuffer.Activate(Constants.SCENE_BUFFER_INDEX);

            foreach (var renderer in renderers)
            {
                var worldMatrix = renderer.transform.LocalToWorldMatrix;
                worldMatrix.Transpose();
                objectBuffer.UpdateBuffer(new ObjectBuffer() { WorldMatrix = worldMatrix });
                objectBuffer.Activate(Constants.OBJECT_BUFFER_INDEX);

                renderer.shader.Activate();

                renderer.mesh.VerticesBuffer.Activate(0);
                renderer.mesh.IndicesBuffer.Activate(0);

                engine.Device.DrawIndexed(renderer.mesh.IndexCount, 0, 0);
            }

            engine.Device.RenderBlitBuffer();
        }

        private void UpdateSceneBuffer()
        {
            var camera = cameraManager.MainCamera;
            var proj = Matrix.PerspectiveFovRH(camera.FOVRad, 1, camera.NearClip, camera.FarClip);
            proj.Transpose();
            var vm = camera.Transform.WorldToLocalMatrix;
            vm.Transpose();
            sceneData = new SceneBuffer()
            {
                ViewMatrix = vm,
                ProjectionMatrix = proj
            };
        }

        public RenderHandle RegisterRenderer(MeshHandle mesh, ShaderHandle materialHandle, Transform transform)
        {
            RendererEntry entry = new RendererEntry()
            {
                mesh = engine.meshManager.GetMeshByHandle(mesh),
                shader = engine.shaderManager.GetShaderByHandle(materialHandle),
                transform = transform
            };

            renderers.Add(entry);

            return new RenderHandle();
        }

        public void UnregisterRenderer(RenderHandle renderHandle)
        {
            
        }
    }

    internal class RendererEntry
    {
        public Mesh mesh;
        public Shader shader;
        public Transform transform;
    }
}
