using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheEngine.Interfaces;
using TheMaths;

namespace TheEngine.Entities
{
    public class Camera : ICamera
    {
        public Transform Transform { get; }

        // FOV in radians
        public float FOVRad { get; set; }
        public float FOV
        {
            get => MathUtil.RadiansToDegrees(FOVRad);
            set => FOVRad = MathUtil.DegreesToRadians(value);
        }
        public float NearClip { get; set; }
        public float FarClip { get; set; }

        public Camera()
        {
            Transform = new Transform();
            FOV = 40;
            NearClip = 1f;
            FarClip = 36600f;
        }
    }
}
