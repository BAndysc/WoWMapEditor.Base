using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheDX11.Resources;
using TheEngine.Handles;

namespace TheEngine.Entities
{
    public class Material
    {
        private readonly Engine engine;
        private readonly ShaderHandle shaderHandle;

        private Shader shader;
        
        internal Shader Shader => shader;

        public ShaderHandle ShaderHandle => shaderHandle;

        internal Dictionary<int, ITexture> textures { get; }
        internal Dictionary<int, INativeBuffer> structuredBuffers { get; }
        internal Dictionary<int, INativeBuffer> structuredVertexBuffers { get; }
        internal Dictionary<int, INativeBuffer> structuredPixelsBuffers { get; }

        internal Material(Engine engine, ShaderHandle shaderHandle)
        {
            this.engine = engine;
            this.shaderHandle = shaderHandle;
            this.shader = engine.shaderManager.GetShaderByHandle(shaderHandle);

            textures = new Dictionary<int, ITexture>();
            structuredBuffers = new Dictionary<int, INativeBuffer>();
            structuredVertexBuffers = new Dictionary<int, INativeBuffer>();
            structuredPixelsBuffers = new Dictionary<int, INativeBuffer>();
        }

        public void SetStructuredBuffer<T>(int index, T[] data, StructuredBufferMode mode = StructuredBufferMode.VertexPixel) where T : struct
        {
            var bufferMode = BufferTypeEnum.StructuredBuffer;
            if (mode == StructuredBufferMode.PixelOnly)
                bufferMode = BufferTypeEnum.StructuredBufferPixelOnly;
            else if (mode == StructuredBufferMode.VertexOnly)
                bufferMode = BufferTypeEnum.StructuredBufferVertexOnly;

            INativeBuffer buffer = engine.Device.CreateBuffer<T>(bufferMode, data);

            if (mode == StructuredBufferMode.PixelOnly)
            {
                structuredPixelsBuffers[index] = buffer;
            }
            else if (mode == StructuredBufferMode.VertexOnly)
            {
                structuredVertexBuffers[index] = buffer;
            }
            else
            {
                structuredBuffers[index] = buffer;
            }
        }
        
        public void SetTexture(int index, TextureHandle texture)
        {
            textures[index] = engine.textureManager.GetTextureByHandle(texture);
        }

        public enum StructuredBufferMode
        {
            VertexOnly,
            PixelOnly,
            VertexPixel
        }
    }
}
