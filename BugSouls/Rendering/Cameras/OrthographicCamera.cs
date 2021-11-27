using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls.Rendering.Cameras
{
    internal class OrthographicCamera : Camera
    {
        public float Width
        {
            get => width;
            set
            {
                width = value;
                SetProjectionChanged();
            }
        }

        public float Height
        {
            get => height;
            set
            {
                height = value;
                SetProjectionChanged();
            }
        }

        private float width;
        private float height;

        public OrthographicCamera(float width, float height, float zNear, float zFar) : base(zNear, zFar)
        {
            this.width = width;
            this.height = height;
        }

        protected override Matrix4 UpdateProjection()
        {
            return Matrix4.CreateOrthographic(Width, height, ZNear, ZFar);
        }
    }
}
