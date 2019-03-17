using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheMaths;

namespace TheEngine.Entities
{
    public class Transform
    {
        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }
        public Quaternion Rotation { get; set; }

        public Transform()
        {
            Position = Vector3.Zero;
            Rotation = Quaternion.Identity;
            Scale = Vector3.One;
        }

        public Matrix LocalToWorldMatrix => Matrix.Transformation(Vector3.Zero, Quaternion.Identity, Scale, Vector3.Zero, Rotation, Position);
        public Matrix WorldToLocalMatrix => Matrix.Invert(LocalToWorldMatrix);
    }
}
