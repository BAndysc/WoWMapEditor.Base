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

        private ITexture[] textures;

        internal Shader Shader => shader;
        internal ITexture[] Textures => textures;

        internal Material(Engine engine, ShaderHandle shaderHandle)
        {
            textures = new ITexture[1]; // @todo

            this.engine = engine;
            this.shaderHandle = shaderHandle;
            this.shader = engine.shaderManager.GetShaderByHandle(shaderHandle);
        }

        public void SetTexture(int index, TextureHandle texture)
        {
            textures[index] = engine.textureManager.GetTextureByHandle(texture);
        }
    }
}
