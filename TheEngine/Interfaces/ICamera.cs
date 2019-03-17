using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheEngine.Entities;

namespace TheEngine.Interfaces
{
    public interface ICamera
    {
        Transform Transform { get; }
        float FOV { get; set; }
        float FOVRad { get; set; }

        float NearClip { get; set; }
        float FarClip { get; set; }
    }
}
