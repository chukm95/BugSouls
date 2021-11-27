using BugSouls.ResourceManagement.Shaders;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls.Rendering.Cameras
{
    internal abstract class Camera
    {
        public float ZNear
        {
            get => zNear;
            set
            {
                zNear = value;
                hasProjectionChanged = true;
            }
        }

        public float ZFar
        {
            get => zFar;
            set
            {
                zFar = value;
                hasProjectionChanged = true;
            }
        }

        public Vector3 Position
        {
            get => position;
            set
            {
                position = value;
                hasViewChanged = true;
            }
        }

        public float PosX
        {
            get => position.X;
            set
            {
                position.X = value;
                hasViewChanged = true;
            }
        }

        public float PosY
        {
            get => position.Y;
            set
            {
                position.Y = value;
                hasViewChanged = true;
            }
        }

        public float PosZ
        {
            get => position.Z;
            set
            {
                position.Z = value;
                hasViewChanged = true;
            }
        }

        public Vector3 Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                hasViewChanged = true;
            }
        }

        public float RotX
        {
            get => rotation.X;
            set
            {
                rotation.X = value;
                hasViewChanged = true;
            }
        }

        public float RotY
        {
            get => rotation.Y;
            set
            {
                rotation.Y = value;
                hasViewChanged = true;
            }
        }

        public float RotZ
        {
            get => rotation.Z;
            set
            {
                rotation.Z = value;
                hasViewChanged = true;
            }
        }

        public Matrix4 View
        {
            get => viewMatrix;
        }

        public Matrix4 Projecion
        {
            get => projectionMatrix;
        }

        public Matrix4 ViewProjection
        {
            get => viewProjectionMatrix;
        }

        private float zNear;
        private float zFar;
        private Vector3 position;
        private Vector3 rotation;
        private Matrix4 viewMatrix;
        private bool hasViewChanged;
        private Matrix4 projectionMatrix;
        private bool hasProjectionChanged;
        private Matrix4 viewProjectionMatrix;

        protected Camera(float zNear, float zFar)
        {
            this.zNear = zNear;
            this.zFar = zFar;
            position = Vector3.Zero;
            rotation = Vector3.Zero;
            viewMatrix = Matrix4.Identity;
            hasViewChanged = true;
            projectionMatrix = Matrix4.Identity;
            hasProjectionChanged = true;
        }

        public void Update()
        {
            if (hasViewChanged || hasProjectionChanged)
            {
                if (hasViewChanged)
                    viewMatrix = UpdateView();
                if (hasProjectionChanged)
                    projectionMatrix = UpdateProjection();

                viewProjectionMatrix = viewMatrix * projectionMatrix;
                hasViewChanged = false;
                hasProjectionChanged = false;
            }
        }

        private Matrix4 UpdateView()
        {
            Quaternion rotationQuat = Quaternion.FromEulerAngles(rotation).Inverted();
            return Matrix4.LookAt(position, position + Vector3.Transform(-Vector3.UnitZ, rotationQuat), Vector3.UnitY);
        }

        protected abstract Matrix4 UpdateProjection();

        protected void SetProjectionChanged()
        {
            hasProjectionChanged = true;
        }

    }
}
