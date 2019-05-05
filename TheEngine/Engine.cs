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
        
        internal TextureManager textureManager { get; }
        public ITextureManager TextureManager => textureManager;

        internal MaterialManager materialManager { get; }
        public IMaterialManager MaterialManager => materialManager;

        private Thread renderThread;

        internal IHwndHost WindowHost => windowHost;

        private volatile bool isDisposing;
        private readonly IHwndHost windowHost;
        private readonly Action onStart;
        private readonly Action<float> updateLoop;

        public double TotalTime;

        public Engine(IConfiguration configuration, IHwndHost windowHost, Action onStart, Action<float> updateLoop)
        {
            windowHost.Bind(this);

            Configuration = configuration;
            this.windowHost = windowHost;
            this.onStart = onStart;
            this.updateLoop = updateLoop;
            Device = new TheDevice(windowHost.Handle, false);

            Device.Initialize();

            lightManager = new LightManager(this);
            inputManager = new InputManager(this);
            cameraManger = new CameraManager(this);

            materialManager = new MaterialManager(this);
            shaderManager = new ShaderManager(this);
            meshManager = new MeshManager(this);
            renderManager = new RenderManager(this);

            textureManager = new TextureManager(this);
        }
        
        public void Start()
        {
            renderThread = new Thread(RenderThread);
            renderThread.Start();
        }

        private void RenderThread()
        {
            onStart();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            double lastMs = 0;
            while (!isDisposing)
            {
                TotalTime = sw.Elapsed.TotalMilliseconds;
                shaderManager.Update();
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

            materialManager.Dispose();
            textureManager.Dispose();
            lightManager.Dispose();
            cameraManger.Dispose();
            renderManager.Dispose();
            shaderManager.Dispose();
            meshManager.Dispose();
            Device.Dispose();
        }
    }
}
