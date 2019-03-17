using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheMaths;

namespace TheEngine.Interfaces
{
    public interface IMesh
    {
        void SetVertices(Vector3[] vertices);
        void SetIndices(int[] indices, int submesh);
        void Rebuild();
    }
}
