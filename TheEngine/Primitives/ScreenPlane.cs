using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheEngine.Data;
using TheMaths;

namespace TheEngine.Primitives
{
    public class ScreenPlane : MeshData
    {
        private static Vector3[] VERTICES = {
            new Vector3(-1, -1, 0),
            new Vector3(-1, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(1, -1, 0),
        };


        private static int[] indices =
        {
            0, 1, 2,
            2, 3, 0
        };

        public ScreenPlane() : base(VERTICES, null, null, indices)
        {

        }
    }
}
