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
        private NativeBuffer<PixelShaderSceneBuffer> pixelShaderSceneBuffer;
        private NativeBuffer<Matrix> instancesBuffer;

        private ObjectBuffer objectData;
        private SceneBuffer sceneData;
        private PixelShaderSceneBuffer scenePixelData;
        private Matrix[] instancesArray;

        private ICameraManager cameraManager;
        
        private Dictionary<Shader, Dictionary<Material, Dictionary<Mesh, List<Transform>>>> renderers;

        private Sampler defaultSampler;

        internal RenderManager(Engine engine)
        {
            this.engine = engine;

            cameraManager = engine.CameraManager;

            sceneBuffer = engine.Device.CreateBuffer<SceneBuffer>(BufferTypeEnum.ConstVertex, 1);
            objectBuffer = engine.Device.CreateBuffer<ObjectBuffer>(BufferTypeEnum.ConstVertex, 1);
            pixelShaderSceneBuffer = engine.Device.CreateBuffer<PixelShaderSceneBuffer>(BufferTypeEnum.ConstPixel, 1);
            instancesBuffer = engine.Device.CreateBuffer<Matrix>(BufferTypeEnum.Vertex, 1);

            instancesArray = new Matrix[1];

            renderers = new Dictionary<Shader, Dictionary<Material, Dictionary<Mesh, List<Transform>>>>();

            sceneData = new SceneBuffer();

            defaultSampler = engine.Device.CreateSampler();
        }
        
        public void Dispose()
        {
            defaultSampler.Dispose();
            instancesBuffer.Dispose();
            pixelShaderSceneBuffer.Dispose();
            objectBuffer.Dispose();
            sceneBuffer.Dispose();
        }

        internal void Render()
        {
            UpdateSceneBuffer();

            engine.Device.RenerClearBuffer();

            sceneBuffer.UpdateBuffer(ref sceneData);
            sceneBuffer.Activate(Constants.SCENE_BUFFER_INDEX);

            pixelShaderSceneBuffer.UpdateBuffer(ref scenePixelData);
            pixelShaderSceneBuffer.Activate(Constants.PIXEL_SCENE_BUFFER_INDEX);

            objectBuffer.Activate(Constants.OBJECT_BUFFER_INDEX);

            defaultSampler.Activate(Constants.DEFAULT_SAMPLER);

            foreach (var shaderPair in renderers)
            {
                shaderPair.Key.Activate();

                foreach (var materialPair in shaderPair.Value)
                {
                    for (int i = 0; i < materialPair.Key.Textures.Length; ++i)
                        materialPair.Key.Textures[i].Activate(i);

                    foreach (var meshPair in materialPair.Value)
                    {
                        meshPair.Key.VerticesBuffer.Activate(0);
                        meshPair.Key.IndicesBuffer.Activate(0);

                        if (shaderPair.Key.Instancing)
                        {
                            if (instancesArray.Length != meshPair.Value.Count)
                                instancesArray = new Matrix[meshPair.Value.Count];

                            for (int i = 0; i < meshPair.Value.Count; ++i)
                            {
                                instancesArray[i] = meshPair.Value[i].LocalToWorldMatrix;
                            }
                            instancesBuffer.UpdateBuffer(instancesArray);
                            instancesBuffer.Activate(1);

                            engine.Device.DrawIndexedInstanced(meshPair.Key.IndexCount, meshPair.Value.Count, 0, 0, 0);
                        }
                        else
                        {
                            foreach (var instance in meshPair.Value)
                            {
                                Matrix.Transpose(ref instance.LocalToWorldMatrix, out objectData.WorldMatrix);
                                objectBuffer.UpdateBuffer(ref objectData);
                                engine.Device.DrawIndexed(meshPair.Key.IndexCount, 0, 0);
                            }
                        }


                    }
                }
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

            sceneData.ViewMatrix = vm;
            sceneData.ProjectionMatrix = proj;
            sceneData.Time = (float)engine.TotalTime;

            scenePixelData.LightDirection = new Vector4(engine.lightManager.MainLight.LightDirection, 0);
            scenePixelData.LightColor = engine.lightManager.MainLight.LightColor;
            scenePixelData.Time = (float)engine.TotalTime;
        }

        public RenderHandle RegisterRenderer(MeshHandle meshHandle, Material material, Transform transform)
        {
            var mesh = engine.meshManager.GetMeshByHandle(meshHandle);
            var shader = material.Shader;

            if (!renderers.ContainsKey(shader))
                renderers[shader] = new Dictionary<Material, Dictionary<Mesh, List<Transform>>>();

            if (!renderers[shader].ContainsKey(material))
                renderers[shader][material] = new Dictionary<Mesh, List<Transform>>();

            if (!renderers[shader][material].ContainsKey(mesh))
                renderers[shader][material][mesh] = new List<Transform>();

            renderers[shader][material][mesh].Add(transform);

            return new RenderHandle();
        }

        public void UnregisterRenderer(RenderHandle renderHandle)
        {
            
        }
    }
}
