using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheDX11.Resources
{
    public class Texture : IDisposable, ITexture
    {
        private readonly Device device;

        internal ShaderResourceView TextureResource { get; }

        internal Texture2D DxTexture { get; }

        public int Width { get; }

        public int Height { get; }

        internal Texture(Device device, int[][] pixels, int width, int height)
        {
            this.device = device;
            Width = width;
            Height = height;

            int stride = Width * 4;

            List<SharpDX.DataRectangle> rectangles = new List<SharpDX.DataRectangle>();
            if (pixels == null)
            {
                pixels = new int[][] { new int[width * height] };
            }
            unsafe
            {
                foreach (int[] mip in pixels)
                {
                    fixed (int* p = mip)
                    {
                        IntPtr pp = (IntPtr)p;
                        rectangles.Add(new SharpDX.DataRectangle(pp, stride));
                        stride /= 2;
                    }
                }
            }
            
            DxTexture = new Texture2D(device, new Texture2DDescription()
            {
                Width = Width,
                Height = Height,
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
                Usage = ResourceUsage.Default,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                MipLevels = rectangles.Count,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
            }, rectangles.ToArray());

            ShaderResourceViewDescription srvDesc = new ShaderResourceViewDescription()
            {
                Format = DxTexture.Description.Format,
                Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D,
            };
            srvDesc.Texture2D.MostDetailedMip = 0;
            srvDesc.Texture2D.MipLevels = -1;

            TextureResource = new ShaderResourceView(device, DxTexture, srvDesc);
        }

        // Call from render thread only
        public void Activate(int slot)
        {
            device.ImmediateContext.PixelShader.SetShaderResource(slot, TextureResource);
        }

        public void Dispose()
        {
            TextureResource.Dispose();
            DxTexture.Dispose();
        }
    }
}
