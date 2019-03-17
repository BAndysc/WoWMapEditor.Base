using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheEngine.Entities;
using TheEngine.Handles;

namespace TheEngine.Interfaces
{
    public interface IRenderManager
    {
        RenderHandle RegisterRenderer(MeshHandle mesh, ShaderHandle materialHandle, Transform transform);
        void UnregisterRenderer(RenderHandle renderHandle);
    }
}
