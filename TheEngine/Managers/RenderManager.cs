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
using TheEngine.Primitives;
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
        
        private Dictionary<ShaderHandle, Dictionary<Material, Dictionary<Mesh, List<Transform>>>> renderers;

        private Sampler defaultSampler;

        private DepthStencil depthStencilZWrite;
        private DepthStencil depthStencilNoZWrite;

        private bool? currentZwrite;

        private RenderTexture renderTexture;

        private RenderTexture outlineTexture;

        private IMesh planeMesh;

        private ShaderHandle blitShader;

        private Material blitMaterial;

        private Material unlitMaterial;

        private Mesh currentMesh = null;

        private Shader currentShader = null;

        internal RenderManager(Engine engine)
        {
            this.engine = engine;

            cameraManager = engine.CameraManager;

            sceneBuffer = engine.Device.CreateBuffer<SceneBuffer>(BufferTypeEnum.ConstVertex, 1);
            objectBuffer = engine.Device.CreateBuffer<ObjectBuffer>(BufferTypeEnum.ConstVertex, 1);
            pixelShaderSceneBuffer = engine.Device.CreateBuffer<PixelShaderSceneBuffer>(BufferTypeEnum.ConstPixel, 1);
            instancesBuffer = engine.Device.CreateBuffer<Matrix>(BufferTypeEnum.Vertex, 1);

            instancesArray = new Matrix[1];

            renderers = new Dictionary<ShaderHandle, Dictionary<Material, Dictionary<Mesh, List<Transform>>>>();

            sceneData = new SceneBuffer();

            defaultSampler = engine.Device.CreateSampler();

            depthStencilZWrite = engine.Device.CreateDepthStencilState(true);
            depthStencilNoZWrite = engine.Device.CreateDepthStencilState(false);

            renderTexture = engine.Device.CreateRenderTexture((int)engine.WindowHost.WindowWidth, (int)engine.WindowHost.WindowHeight);

            outlineTexture = engine.Device.CreateRenderTexture((int)engine.WindowHost.WindowWidth, (int)engine.WindowHost.WindowHeight);

            planeMesh = engine.MeshManager.CreateMesh(new ScreenPlane());

            blitShader = engine.ShaderManager.LoadShader("../internalShaders/blit.shader");
            blitMaterial = engine.MaterialManager.CreateMaterial(blitShader);

            unlitMaterial = engine.MaterialManager.CreateMaterial(engine.ShaderManager.LoadShader("../internalShaders/unlit.shader"));
        }

        private void SetZWrite(bool zwrite)
        {
            if (!currentZwrite.HasValue || currentZwrite.Value != zwrite)
            {
                if (zwrite)
                    depthStencilZWrite.Activate();
                else
                    depthStencilNoZWrite.Activate();
                currentZwrite = zwrite;
            }
        }

        public void Dispose()
        {
            outlineTexture.Dispose();
            renderTexture.Dispose();

            depthStencilZWrite.Dispose();
            depthStencilNoZWrite.Dispose();
            defaultSampler.Dispose();
            instancesBuffer.Dispose();
            pixelShaderSceneBuffer.Dispose();
            objectBuffer.Dispose();
            sceneBuffer.Dispose();
        }
        
        internal void Render()
        {
            UpdateSceneBuffer();

            engine.Device.RenderClearBuffer();

            engine.Device.SetRenderTexture(renderTexture);
            renderTexture.Clear(1, 1, 1, 1);

            sceneBuffer.UpdateBuffer(ref sceneData);
            sceneBuffer.Activate(Constants.SCENE_BUFFER_INDEX);

            pixelShaderSceneBuffer.UpdateBuffer(ref scenePixelData);
            pixelShaderSceneBuffer.Activate(Constants.PIXEL_SCENE_BUFFER_INDEX);

            objectBuffer.Activate(Constants.OBJECT_BUFFER_INDEX);

            defaultSampler.Activate(Constants.DEFAULT_SAMPLER);

            RenderAll(null);

            engine.Device.RenderClearBuffer();
            engine.Device.SetRenderTexture(outlineTexture);
            outlineTexture.Clear(0, 0, 0, 0);

            RenderAll(unlitMaterial);

            engine.Device.SetRenderTexture(null);

            blitMaterial.Shader.Activate();
            SetZWrite(false);
            renderTexture.Activate(0);
            outlineTexture.Activate(1);
            engine.meshManager.GetMeshByHandle(planeMesh.Handle).IndicesBuffer.Activate(0);
            engine.meshManager.GetMeshByHandle(planeMesh.Handle).VerticesBuffer.Activate(0);
            engine.Device.DrawIndexed(engine.meshManager.GetMeshByHandle(planeMesh.Handle).IndexCount, 0, 0);


            engine.Device.RenderBlitBuffer();
        }

        private void RenderAll(Material overrideMaterial)
        {
            currentMesh = null;
            currentShader = null;

            foreach (var shaderPair in renderers)
            {
                if (!engine.shaderManager.GetShaderByHandle(shaderPair.Key).WriteMask && overrideMaterial != null)
                    continue;

                if (overrideMaterial != null)
                    currentShader = engine.shaderManager.GetShaderByHandle(overrideMaterial.ShaderHandle);
                else
                    currentShader = engine.shaderManager.GetShaderByHandle(shaderPair.Key);
                currentShader.Activate();

                foreach (var materialPair in shaderPair.Value)
                {
                    SetZWrite(materialPair.Key.Shader.ZWrite);

                    foreach (var texturePair in materialPair.Key.textures)
                        texturePair.Value.Activate(texturePair.Key);

                    if (overrideMaterial == null)
                    {
                        foreach (var bufferKeyPair in materialPair.Key.structuredBuffers)
                            bufferKeyPair.Value.Activate(bufferKeyPair.Key);

                        foreach (var bufferKeyPair in materialPair.Key.structuredPixelsBuffers)
                            bufferKeyPair.Value.Activate(bufferKeyPair.Key);

                        foreach (var bufferKeyPair in materialPair.Key.structuredVertexBuffers)
                            bufferKeyPair.Value.Activate(bufferKeyPair.Key);
                    }

                    foreach (var meshPair in materialPair.Value)
                    {
                        if (currentMesh != meshPair.Key)
                        {
                            meshPair.Key.VerticesBuffer.Activate(0);
                            meshPair.Key.IndicesBuffer.Activate(0);
                            currentMesh = meshPair.Key;
                        }

                        if (currentShader.Instancing)
                        {
                            if (instancesArray.Length < meshPair.Value.Count)
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
                                objectData.WorldMatrix = instance.LocalToWorldMatrix;
                                objectBuffer.UpdateBuffer(ref objectData);
                                engine.Device.DrawIndexed(meshPair.Key.IndexCount, 0, 0);
                            }
                        }
                    }
                }
            }
        }

        private void UpdateSceneBuffer()
        {
            var camera = cameraManager.MainCamera;
            var proj = Matrix.PerspectiveFovRH(camera.FOVRad, engine.WindowHost.Aspect, camera.NearClip, camera.FarClip);
            var vm = camera.Transform.WorldToLocalMatrix;

            sceneData.ViewMatrix = vm;
            sceneData.ProjectionMatrix = proj;
            sceneData.LightPosition = engine.lightManager.MainLight.LightPosition;
            sceneData.CameraPosition = new Vector4(engine.CameraManager.MainCamera.Transform.Position, 1);
            sceneData.Time = (float)engine.TotalTime;

            scenePixelData.LightDirection = new Vector4(engine.lightManager.MainLight.LightDirection, 0);
            scenePixelData.LightColor = engine.lightManager.MainLight.LightColor;
            scenePixelData.LightPosition = engine.lightManager.MainLight.LightPosition;
            scenePixelData.Time = (float)engine.TotalTime;
            scenePixelData.ScreenWidth = engine.WindowHost.WindowWidth;
            scenePixelData.ScreenHeight = engine.WindowHost.WindowHeight;
        }

        public RenderHandle RegisterRenderer(MeshHandle meshHandle, Material material, Transform transform)
        {
            var mesh = engine.meshManager.GetMeshByHandle(meshHandle);
            var shader = material.ShaderHandle;

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
