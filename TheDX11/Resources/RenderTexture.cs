using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheDX11.Resources
{
    public class RenderTexture : IDisposable, ITexture
    {
        private Texture underlyingTexture;

        internal RenderTargetView TargetView;

        private readonly Device device;

        internal RenderTexture(Device device, int width, int height) : this(device, new Texture(device, null, width, height))
        {
            RenderTargetViewDescription description = new RenderTargetViewDescription()
            {
                Format = underlyingTexture.DxTexture.Description.Format,
                Dimension = RenderTargetViewDimension.Texture2D,
                Texture2D = new RenderTargetViewDescription.Texture2DResource()
                {
                    MipSlice = 0
                }
            };

            TargetView = new RenderTargetView(device, underlyingTexture.DxTexture, description);
            this.device = device;
        }

        internal RenderTexture(Device device, Texture texture)
        {
            underlyingTexture = texture;

            RenderTargetViewDescription description = new RenderTargetViewDescription()
            {
                Format = underlyingTexture.DxTexture.Description.Format,
                Dimension = RenderTargetViewDimension.Texture2D,
                Texture2D = new RenderTargetViewDescription.Texture2DResource()
                {
                    MipSlice = 0
                }
            };

            TargetView = new RenderTargetView(device, underlyingTexture.DxTexture, description);
        }

        public int Width => underlyingTexture.Width;

        public int Height => underlyingTexture.Height;

        public void Clear(float r, float g, float b, float a)
        {
            device.ImmediateContext.ClearRenderTargetView(TargetView, new Color4(r, g, b, a));
        }

        public void Activate(int slot)
        {
            underlyingTexture.Activate(slot);
        }

        public void Dispose()
        {
            TargetView.Dispose();
            underlyingTexture.Dispose();
        }
    }
}
