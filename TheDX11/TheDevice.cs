using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheDX11.Resources;
using TheDX11.WinApi;
using Device = SharpDX.Direct3D11.Device;
using Resource = SharpDX.Direct3D11.Resource;

namespace TheDX11
{
    public class TheDevice : IDisposable
    {
        private readonly IntPtr outputHandle;
        private readonly bool dxDebug;

        private Device device;
        private SwapChain swapChain;
        private RenderTargetView renderTargetView;
        private Texture2D depthStencilBuffer;
        private DepthStencilState depthStencilState;
        private DepthStencilView depthStencilView;
        private RasterizerState rasterizerState;

        public TheDevice(IntPtr outputHandle, bool dxDebug)
        {
            this.outputHandle = outputHandle;
            this.dxDebug = dxDebug;
        }

        public void Initialize()
        {
            var flags = DeviceCreationFlags.BgraSupport;
            if (dxDebug)
                flags |= DeviceCreationFlags.Debug;

            NativeMethods.GetWindowRect(outputHandle, out var size);
            
            var swapChainDescription = new SwapChainDescription
            {
                OutputHandle = outputHandle,
                BufferCount = 1,
                Flags = SwapChainFlags.AllowModeSwitch,
                IsWindowed = true,
                ModeDescription = new ModeDescription(size.Width, size.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            Device.CreateWithSwapChain(DriverType.Hardware, flags, swapChainDescription, out device, out swapChain);


            // New RenderTargetView from the backbuffer
            var backBuffer = Resource.FromSwapChain<Texture2D>(swapChain, 0);
            renderTargetView = new RenderTargetView(device, backBuffer);
            backBuffer.Dispose();


            Texture2DDescription depthBufferDesc = new Texture2DDescription()
            {
                Width = size.Width,
                Height = size.Height,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.D24_UNorm_S8_UInt,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            };
            depthStencilBuffer = new Texture2D(device, depthBufferDesc);

            DepthStencilStateDescription depthStencilDesc = new DepthStencilStateDescription()
            {
                IsDepthEnabled = true,
                DepthWriteMask = DepthWriteMask.All,
                DepthComparison = Comparison.Less,
                IsStencilEnabled = true,
                StencilReadMask = 0xFF,
                StencilWriteMask = 0xFF,
                FrontFace = new DepthStencilOperationDescription()
                {
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Increment,
                    PassOperation = StencilOperation.Keep,
                    Comparison = Comparison.Always
                },
                BackFace = new DepthStencilOperationDescription()
                {
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Decrement,
                    PassOperation = StencilOperation.Keep,
                    Comparison = Comparison.Always
                }
            };
            depthStencilState = new DepthStencilState(device, depthStencilDesc);

            DepthStencilViewDescription depthStencilViewDesc = new DepthStencilViewDescription()
            {
                Format = Format.D24_UNorm_S8_UInt,
                Dimension = DepthStencilViewDimension.Texture2D,
                Texture2D = new DepthStencilViewDescription.Texture2DResource()
                {
                    MipSlice = 0
                }
            };
            depthStencilView = new DepthStencilView(device, depthStencilBuffer, depthStencilViewDesc);

            RasterizerStateDescription rasterDesc = new RasterizerStateDescription()
            {
                IsAntialiasedLineEnabled = false,
                CullMode = CullMode.None,
                DepthBias = 0,
                DepthBiasClamp = .0f,
                IsDepthClipEnabled = true,
                FillMode = FillMode.Solid, //FillMode.Wireframe,
                IsFrontCounterClockwise = false,
                IsMultisampleEnabled = false,
                IsScissorEnabled = false,
                SlopeScaledDepthBias = .0f
            };
            rasterizerState = new RasterizerState(device, rasterDesc);


            device.ImmediateContext.Rasterizer.SetViewport(0, 0, size.Width, size.Height);
            device.ImmediateContext.OutputMerger.SetDepthStencilState(depthStencilState, 1);
            device.ImmediateContext.OutputMerger.SetTargets(depthStencilView, renderTargetView);
            device.ImmediateContext.Rasterizer.State = rasterizerState;
        }

        public void DrawIndexed(object indexCount, int v1, int v2)
        {
            throw new NotImplementedException();
        }

        // Safe multithread call
        public Texture CreateTexture(int width, int height, int[][] pixels)
        {
            return new Texture(device, pixels, width, height);
        }

        // Safe multithread call
        public Shader CreateShader(string path, string includePath = null)
        {
            return new Shader(device, path, includePath);
        }

        // Safe multithread call
        public NativeBuffer<T> CreateBuffer<T>(BufferTypeEnum bufferType, int size) where T : struct
        {
            return new NativeBuffer<T>(device, bufferType, size);
        }


        // call only form render thread
        public void RenerClearBuffer()
        {
            device.ImmediateContext.ClearDepthStencilView(depthStencilView, DepthStencilClearFlags.Depth, 1, 0);
            device.ImmediateContext.ClearRenderTargetView(renderTargetView, new Color4(0.4f, 0.4f, 0.4f, 1));
        }

        // call only form render thread
        public void DrawIndexed(int indexCount, int startIndexLocation, int baseVertexLocation)
        {
            device.ImmediateContext.DrawIndexed(indexCount, startIndexLocation, baseVertexLocation);
        }

        // Call only from render thread
        public void RenderBlitBuffer()
        {
            swapChain.Present(0, PresentFlags.None);
        }

        public void Dispose()
        {
            rasterizerState.Dispose();
            depthStencilView.Dispose();
            depthStencilState.Dispose();
            depthStencilBuffer.Dispose();
            renderTargetView.Dispose();
            swapChain.Dispose();
            device.Dispose();
        }
    }
}
