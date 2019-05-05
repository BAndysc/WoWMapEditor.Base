using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheDX11.Resources;
using TheEngine.Handles;
using TheEngine.Interfaces;

namespace TheEngine.Managers
{
    internal class TextureManager : ITextureManager, IDisposable
    {
        private readonly Engine engine;
        private Dictionary<string, TextureHandle> texturesByPath;
        
        private List<ITexture> allTextures;

        internal TextureManager(Engine engine)
        {
            texturesByPath = new Dictionary<string, TextureHandle>();
            allTextures = new List<ITexture>();
            this.engine = engine;
        }

        public void Dispose()
        {
            foreach (var tex in allTextures)
                tex.Dispose();
        }

        public TextureHandle LoadTexture(string path)
        {
            if (texturesByPath.TryGetValue(path, out var handle))
                return handle;

            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(path);
            //bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            int[] data = new int[bitmap.Width * bitmap.Height];

            for (int y = 0; y < bitmap.Height; ++y)
            {
                for (int x = 0; x < bitmap.Width; ++x)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    data[y * bitmap.Width + x] = (pixel.R) | (pixel.G << 8) | (pixel.B << 16);
                }
            }

            var textureHandle = CreateTexture(new int[][]{ data }, bitmap.Width, bitmap.Height);
            bitmap.Dispose();
            texturesByPath.Add(path, textureHandle);
            return textureHandle;
        }

        public TextureHandle CreateTexture(int[][] pixels, int width, int height)
        {
            var texture = engine.Device.CreateTexture(width, height, pixels);
            var textureHandle = new TextureHandle(allTextures.Count);

            allTextures.Add(texture);

            return textureHandle;
        }
        public TextureHandle CreateTextureArray(int[][][] textures, int width, int height)
        {
            var texture = engine.Device.CreateTextureArray(width, height, textures);
            var textureHandle = new TextureHandle(allTextures.Count);

            allTextures.Add(texture);

            return textureHandle;
        }

        public TextureHandle LoadTextureArray(params string[] paths)
        {
            int[][][] textures = new int[paths.Length][][];

            int width = 0;
            int height = 0;
                        
            for (int i = 0; i < paths.Length; ++i)
            {
                var path = paths[i];

                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(path);
                int[] data = new int[bitmap.Width * bitmap.Height];

                width = bitmap.Width;
                height = bitmap.Height;

                textures[i] = new int[1][];
                textures[i][0] = data;

                for (int y = 0; y < bitmap.Height; ++y)
                {
                    for (int x = 0; x < bitmap.Width; ++x)
                    {
                        var pixel = bitmap.GetPixel(x, y);
                        data[y * bitmap.Width + x] = (pixel.R) | (pixel.G << 8) | (pixel.B << 16);
                    }
                }

                bitmap.Dispose();
            }

            var texture = engine.Device.CreateTextureArray(width, height, textures);
            var textureHandle = new TextureHandle(allTextures.Count);
            
            allTextures.Add(texture);

            return textureHandle;
        }
        public TextureHandle LoadTextureCube(params string[] paths)
        {
            if (paths.Length != 6)
                throw new Exception();
            int[][] textures = new int[paths.Length][];

            int width = 0;
            int height = 0;

            for (int i = 0; i < paths.Length; ++i)
            {
                var path = paths[i];

                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(path);
                width = bitmap.Width;
                height = bitmap.Height;

                textures[i] = new int[bitmap.Width * bitmap.Height];

                for (int y = 0; y < bitmap.Height; ++y)
                {
                    for (int x = 0; x < bitmap.Width; ++x)
                    {
                        var pixel = bitmap.GetPixel(x, y);
                        textures[i][y * bitmap.Width + x] = (pixel.R) | (pixel.G << 8) | (pixel.B << 16);
                    }
                }

                bitmap.Dispose();
            }

            var texture = engine.Device.CreateTextureCube(width, height, textures);
            var textureHandle = new TextureHandle(allTextures.Count);

            allTextures.Add(texture);

            return textureHandle;
        }

        internal ITexture GetTextureByHandle(TextureHandle textureHandle)
        {
            return allTextures[textureHandle.Handle];
        }
    }
}
