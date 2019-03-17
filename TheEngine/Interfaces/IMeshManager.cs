using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheMaths;

namespace TheEngine.Interfaces
{
    public interface IMeshManager
    {
        IMesh CreateMesh(Vector3[] vertices, int[] indices);
    }
}
