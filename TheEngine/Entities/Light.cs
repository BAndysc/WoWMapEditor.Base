using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheMaths;

namespace TheEngine.Entities
{
    public class DirectionalLight
    {
        public Vector4 LightColor { get; set; }
        public Vector3 LightDirection { get; set; }
    }
}
