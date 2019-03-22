using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheDX11.Resources
{
    public class TextureArray : IDisposable, ITexture
    {
        private readonly Device device;

        internal ShaderResourceView TextureResource { get; }

        public int Width { get; }

        public int Height { get; }

        private Texture2D texture { get; }

        internal TextureArray(Device device, int width, int height, int[][][] pixels)
        {
            this.device = device;
            Width = width;
            Height = height;
                        
            int mipsCount = pixels[0].Length;
            int texturesCount = pixels.Length;

            int[][] rawDatas = new int[mipsCount][];
            SharpDX.DataBox[] boxes = new SharpDX.DataBox[texturesCount * mipsCount];
            unsafe
            {
                for (int j = 0; j < texturesCount; ++j)
                {
                    width = Width;
                    height = Height;
                    for (int i = 0; i < mipsCount; ++i)
                    {
                        fixed (int* p = pixels[j][i])
                        {
                            IntPtr pp = (IntPtr)p;
                            boxes[j * mipsCount + i] = new SharpDX.DataBox(pp, width * 4, width * height * 4);
                        }
                        width /= 2;
                        height /= 2;
                    }
                }
            }

            texture = new Texture2D(device, new Texture2DDescription()
            {
                Width = Width,
                Height = Height,
                ArraySize = texturesCount,
                BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
                Usage = ResourceUsage.Default,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                MipLevels = mipsCount,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
            }, boxes);

            ShaderResourceViewDescription srvDesc = new ShaderResourceViewDescription()
            {
                Format = texture.Description.Format,
                Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2DArray,
            };
            srvDesc.Texture2DArray.MostDetailedMip = 0;
            srvDesc.Texture2DArray.MipLevels = -1;
            srvDesc.Texture2DArray.ArraySize = texturesCount;
            srvDesc.Texture2DArray.FirstArraySlice = 0;

            TextureResource = new ShaderResourceView(device, texture, srvDesc);
        }

        public void Activate(int slot)
        {
            device.ImmediateContext.PixelShader.SetShaderResource(slot, TextureResource);
        }

        public void Dispose()
        {
            TextureResource.Dispose();
            texture.Dispose();
        }
    }
}
