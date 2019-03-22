using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheEngine.Entities;
using TheEngine.Handles;
using TheEngine.Interfaces;

namespace TheEngine.Managers
{
    internal class MaterialManager : IMaterialManager, IDisposable
    {
        private Engine engine;

        public MaterialManager(Engine engine)
        {
            this.engine = engine;
        }

        public Material CreateMaterial(ShaderHandle shader)
        {
            return new Material(engine, shader);
        }

        public void Dispose()
        {
            
        }
    }
}
