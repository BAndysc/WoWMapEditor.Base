using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheDX11;
using TheEngine.Interfaces;
using TheEngine.Managers;

namespace TheEngine
{
    public class Engine : IDisposable
    {
        internal TheDevice Device { get; }

        internal IConfiguration Configuration { get; }

        private ShaderManager shaderManager;
        public IShaderManager ShaderManager => shaderManager;


        private MeshManager meshManager;
        public IMeshManager MeshManager => meshManager;


        private RenderManager renderManager;
        public IRenderManager RenderManager => renderManager;

        private Thread renderThread;

        private bool isDisposing;

        public Engine(IConfiguration configuration, IntPtr outputHandle)
        {
            Configuration = configuration;


            Device = new TheDevice(outputHandle, true);

            Device.Initialize();

            shaderManager = new ShaderManager(this);
            meshManager = new MeshManager(this);
            renderManager = new RenderManager(this);
        }
        
        public void Start()
        {
            renderThread = new Thread(RenderThread);
            renderThread.Start();
        }

        private void RenderThread()
        {
            while (!isDisposing)
            {
                renderManager.Render();
            }
        }

        public void Dispose()
        {
            isDisposing = true;
            renderThread.Join();
            renderManager.Dispose();
            shaderManager.Dispose();
            meshManager.Dispose();
            Device.Dispose();
        }
    }
}
