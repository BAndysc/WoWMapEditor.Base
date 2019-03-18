using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheEngine.Entities;
using TheEngine.Interfaces;

namespace TheEngine.Managers
{
    public class LightManager : ILightManager, IDisposable
    {
        private readonly Engine engine;

        public DirectionalLight MainLight { get; set; }

        internal LightManager(Engine engine)
        {
            this.engine = engine;
            MainLight = new DirectionalLight();
            MainLight.LightDirection = new TheMaths.Vector3(-1, 0, 0);
            MainLight.LightColor = new TheMaths.Vector4(1, 1, 1, 1);
        }

        public void Dispose()
        {
            
        }
    }
}
