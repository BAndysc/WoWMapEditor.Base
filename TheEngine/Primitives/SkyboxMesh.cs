using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheEngine.Data;
using TheMaths;

namespace TheEngine.Primitives
{
    public class SkyboxMesh : MeshData
    {
        private static float SIZE = 1f;

        private static Vector3[] VERTICES = {
            new Vector3(-SIZE,  -SIZE, -SIZE),
            new Vector3(-SIZE, -SIZE, SIZE),
            new Vector3(SIZE, -SIZE, SIZE),
            new Vector3(SIZE, -SIZE, -SIZE),

            new Vector3(-SIZE,  SIZE, -SIZE),
            new Vector3(-SIZE, SIZE, SIZE),
            new Vector3(SIZE, SIZE, SIZE),
            new Vector3(SIZE, SIZE, -SIZE),
        };


        private static int[] indices =
        {
            0,3,1,
            1,3,2,

            0,1,4,
            1,5,4,

            7,2,3,
            7,6,2,

            1,2,5,
            5,2,6,

            0,4,3,
            3,4,7,

            4,5,7,
            5,6,7,
        };

        public SkyboxMesh() : base(VERTICES, null, null, indices)
        {

        }
    }
}
