using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheDX11.Resources;
using TheEngine.Config;
using TheEngine.Handles;
using TheEngine.Interfaces;
using TheEngine.Structures;

namespace TheEngine.Managers
{
    public class RenderManager : IRenderManager, IDisposable
    {
        private readonly Engine engine;

        private NativeBuffer<SceneBuffer> sceneBuffer;

        private SceneBuffer sceneData;

        internal RenderManager(Engine engine)
        {
            this.engine = engine;
            sceneBuffer = engine.Device.CreateBuffer<SceneBuffer>(BufferTypeEnum.ConstVertex, 1);

            sceneData = new SceneBuffer();
        }
        
        public void Dispose()
        {
            sceneBuffer.Dispose();
        }

        internal void Render()
        {
            engine.Device.RenerClearBuffer();

            sceneBuffer.UpdateBuffer(new SceneBuffer[] { sceneData });

            sceneBuffer.Activate(Constants.SCENE_BUFFER_INDEX);

            engine.Device.RenderBlitBuffer();
        }

        public RenderHandle RegisterRenderer(MeshHandle mesh, MaterialHandle materialHandle)
        {
            return new RenderHandle();
        }

        public void UnregisterRenderer(RenderHandle renderHandle)
        {
            
        }
    }
}
