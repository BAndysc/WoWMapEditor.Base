﻿using System;
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
        
        private List<Texture> allTextures;

        internal TextureManager(Engine engine)
        {
            texturesByPath = new Dictionary<string, TextureHandle>();
            allTextures = new List<Texture>();
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
            
            var texture = engine.Device.CreateTexture(bitmap.Width, bitmap.Height, new int[][] { data });
            var textureHandle = new TextureHandle(allTextures.Count);
            bitmap.Dispose();

            texturesByPath.Add(path, textureHandle);
            allTextures.Add(texture);

            return textureHandle;
        }

        internal Texture GetTextureByHandle(TextureHandle textureHandle)
        {
            return allTextures[textureHandle.Handle];
        }
    }
}