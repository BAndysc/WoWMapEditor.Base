using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheDX11.Resources
{
    public interface ITexture : IDisposable
    {
        int Width { get; }
        int Height { get; }

        void Activate(int slot);
    }
}
