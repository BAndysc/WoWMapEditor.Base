using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheDX11.Resources
{
    public class DepthStencil : IDisposable
    {
        private readonly Device device;
        private DepthStencilState depthStencilState;

        internal DepthStencil(Device device, bool zWrite)
        {
            DepthStencilStateDescription depthStencilDesc = new DepthStencilStateDescription()
            {
                IsDepthEnabled = true,
                DepthWriteMask = zWrite ? DepthWriteMask.All : DepthWriteMask.Zero,
                DepthComparison = Comparison.Less,
                IsStencilEnabled = false,
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
            this.device = device;
        }

        public void Activate()
        {
            device.ImmediateContext.OutputMerger.SetDepthStencilState(depthStencilState, 1);
        }

        public void Dispose()
        {
            depthStencilState.Dispose();
        }
    }
}
