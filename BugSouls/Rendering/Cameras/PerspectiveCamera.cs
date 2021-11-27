using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls.Rendering.Cameras
{
    internal class PerspectiveCamera : Camera
    {
        public float Fov
        {
            get => fov;
            set
            {
                fov = value;
                SetProjectionChanged();
            }
        }

        public float AspectRatio
        {
            set
            {
                aspectRatio = value;
                SetProjectionChanged();
            }
        }

        private float fov;
        private float aspectRatio;

        public PerspectiveCamera(float fov, float aspectRatio, float znear, float zfar) : base(znear, zfar)
        {
            this.fov = MathHelper.DegreesToRadians(fov);
            this.aspectRatio = aspectRatio;
            ZNear = znear;
            ZFar = zfar;
        }

        protected override Matrix4 UpdateProjection()
        {
            return Matrix4.CreatePerspectiveFieldOfView(fov, aspectRatio, ZNear, ZFar);
        }
    }
}
