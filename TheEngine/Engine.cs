using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        internal ShaderManager shaderManager { get; }
        public IShaderManager ShaderManager => shaderManager;


        internal MeshManager meshManager { get; }
        public IMeshManager MeshManager => meshManager;


        internal RenderManager renderManager { get; }
        public IRenderManager RenderManager => renderManager;


        internal CameraManager cameraManger { get; }
        public ICameraManager CameraManager => cameraManger;


        private Thread renderThread;

        private bool isDisposing;
        private readonly Action<float> updateLoop;

        public Engine(IConfiguration configuration, IntPtr outputHandle, Action<float> updateLoop)
        {
            Configuration = configuration;
            this.updateLoop = updateLoop;
            Device = new TheDevice(outputHandle, true);

            Device.Initialize();

            cameraManger = new CameraManager(this);

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
            Stopwatch sw = new Stopwatch();
            sw.Start();
            double lastMs = 0;
            while (!isDisposing)
            {
                renderManager.Render();
                double sinceLast = sw.Elapsed.TotalMilliseconds - lastMs;
                lastMs = sw.Elapsed.TotalMilliseconds;
                updateLoop((float)sinceLast);
            }
        }

        public void Dispose()
        {
            isDisposing = true;
            renderThread.Join();

            cameraManger.Dispose();
            renderManager.Dispose();
            shaderManager.Dispose();
            meshManager.Dispose();
            Device.Dispose();
        }
    }
}
