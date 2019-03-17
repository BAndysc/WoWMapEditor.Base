using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheEngine.Interfaces;

namespace TheEngine.Config
{
    public class Configuration : IConfiguration
    {
        public string ShaderDirectory { get; private set;  }

        public void SetShaderDirectory(string path)
        {
            ShaderDirectory = path;
        }
    }
}
