using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheDX11.Resources
{
    public class Sampler : IDisposable
    {
        private readonly Device device;
        private SamplerState sampler;

        internal Sampler(Device device)
        {
            SamplerStateDescription samplerDesc = new SamplerStateDescription()
            {
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                MipLodBias = 0,
                MaximumAnisotropy = 1,
                ComparisonFunction = Comparison.Always,
                BorderColor = new Color4(1, 0, 0, 1),
                MinimumLod = 0,
                MaximumLod = float.MaxValue
            };
            sampler = new SamplerState(device, samplerDesc);

            this.device = device;
        }

        // Call from render thread only
        public void Activate(int slot)
        {
            device.ImmediateContext.PixelShader.SetSampler(slot, sampler);
        }

        public void Dispose()
        {
            sampler.Dispose();
        }
    }
}
