using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheEngine.Handles;

namespace TheEngine.Interfaces
{
    public interface IShaderManager
    {
        ShaderHandle LoadShader(string path);
    }
}
