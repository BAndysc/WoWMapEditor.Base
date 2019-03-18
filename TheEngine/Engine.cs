using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheDX11;
using TheEngine.GUI;
using TheEngine.Input;
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

        internal InputManager inputManager { get; }
        public InputManager InputManager => inputManager;

        internal LightManager lightManager { get; }
        public ILightManager LightManager => lightManager;

        private Thread renderThread;

        private volatile bool isDisposing;
        private readonly Action<float> updateLoop;

        public double TotalTime;

        public Engine(IConfiguration configuration, IHwndHost windowHost, Action<float> updateLoop)
        {
            windowHost.Bind(this);

            Configuration = configuration;
            this.updateLoop = updateLoop;
            Device = new TheDevice(windowHost.Handle, true);

            Device.Initialize();

            lightManager = new LightManager(this);
            inputManager = new InputManager(this);
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
                TotalTime = sw.Elapsed.TotalMilliseconds;
                renderManager.Render();
                double sinceLast = sw.Elapsed.TotalMilliseconds - lastMs;
                lastMs = sw.Elapsed.TotalMilliseconds;
                updateLoop((float)sinceLast);

                inputManager.Update();
            }
        }

        public void Dispose()
        {
            isDisposing = true;
            renderThread.Join();

            lightManager.Dispose();
            cameraManger.Dispose();
            renderManager.Dispose();
            shaderManager.Dispose();
            meshManager.Dispose();
            Device.Dispose();
        }
    }
}
