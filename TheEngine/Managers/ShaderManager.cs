using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheDX11.Resources;
using TheEngine.Handles;
using TheEngine.Interfaces;

namespace TheEngine.Managers
{
    public class ShaderManager : IShaderManager, IDisposable
    {
        private Dictionary<string, ShaderHandle> shaderHandles;
        private List<Shader> byHandleShaders;

        private readonly Engine engine;

        internal ShaderManager(Engine engine)
        {
            shaderHandles = new Dictionary<string, ShaderHandle>();
            byHandleShaders = new List<Shader>();
            this.engine = engine;
        }

        public ShaderHandle LoadShader(string path)
        {
            path = engine.Configuration.ShaderDirectory + "/" + path;

            if (shaderHandles.TryGetValue(path, out var shader))
                return shader;

            var newShader = engine.Device.CreateShader(path);

            byHandleShaders.Add(newShader);

            var handle = new ShaderHandle(byHandleShaders.Count - 1);

            shaderHandles.Add(path, handle);

            return handle;
        }

        public void Dispose()
        {
            foreach (var shader in byHandleShaders)
                shader.Dispose();

            byHandleShaders.Clear();
            shaderHandles.Clear();
        }
    }
}
