using BugSouls.Util;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls.Rendering.Cameras
{
    using Window = BugSouls.Util.Window;

    internal class DefaultCamera
    {
        private readonly float FOV = MathHelper.DegreesToRadians(90f);
        private readonly float zNear = 0.1f;
        private readonly float zFar = 20000f;
        private readonly float RotationInRadians = MathHelper.DegreesToRadians(90);
        private readonly float FullRotationRad = MathHelper.DegreesToRadians(360);

        private enum RotationDirection
        {
            NONE,
            LEFT,
            RIGHT
        }

        public Vector3 Position
        {
            get => position_lookAt;
            set
            {
                position_lookAt = value;
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

        private Window window;

        private float angle;
        private float angleTransition;
        private RotationDirection rotationDirection;

        private Vector3 position_lookAt;
        private Vector3 position_camera;

        private Matrix4 projectionMatrix;
        private bool hasProjectionChanged;

        private Matrix4 viewMatrix;
        private bool hasViewChanged;

        private Matrix4 viewProjectionMatrix;

        public DefaultCamera()
        {
            position_lookAt = new Vector3(16f, 0, 16f) * 32f;
            position_camera = new Vector3(0f, 1f, 1.3f) * 32f;

            window = Core.Window;
            window.OnResize += Window_OnResize;

            rotationDirection = RotationDirection.NONE;

            viewMatrix = new Matrix4();
            hasViewChanged = true;

            projectionMatrix = new Matrix4();
            hasProjectionChanged = true;
        }

        private void Window_OnResize(int width, int height)
        {
            hasProjectionChanged = true;
        }

        public void Update(TimeSpan deltaTime)
        {
            if (angle >= 360f)
                angle -= 360f;
            else if (angle < 0f)
                angle += 360f;

            if (rotationDirection == RotationDirection.NONE)
            {
                if (Core.NativeWindow.KeyboardState.IsKeyDown(Keys.Left))
                {
                    rotationDirection = RotationDirection.LEFT;
                    angleTransition = 0;
                }
                else if (Core.NativeWindow.KeyboardState.IsKeyDown(Keys.Right))
                {
                    rotationDirection = RotationDirection.RIGHT;
                    angleTransition = 0;
                }                
            }
            else
            {
                if(rotationDirection == RotationDirection.LEFT)
                    angleTransition -= (float)deltaTime.TotalSeconds * 180f;
                else
                    angleTransition += (float)deltaTime.TotalSeconds * 180f;
                if (Math.Abs(angleTransition) >= 90f)
                {
                    if (rotationDirection == RotationDirection.LEFT)
                    {
                        angle -= 90f;
                        rotationDirection = RotationDirection.NONE;
                    }
                    else if (rotationDirection == RotationDirection.RIGHT)
                    {
                        angle += 90f;
                        rotationDirection = RotationDirection.NONE;
                    }
                    angleTransition = 0;
                }
                hasViewChanged = true;
            }

            if (hasViewChanged || hasProjectionChanged)
            {
                float angleTransWeight = (float)Math.Sin(MathHelper.DegreesToRadians(angleTransition)) * 90f;
                Quaternion quat = Quaternion.FromEulerAngles(0, MathHelper.DegreesToRadians(angle + angleTransWeight) , 0);
                Vector3 camera_pos = Vector3.Transform(position_camera, quat);

                if (hasProjectionChanged)
                {
                    //projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(FOV, window.AspectRatio, zNear, zFar);
                    projectionMatrix = Matrix4.CreateOrthographic(320, 180, zNear, zFar);
                    hasProjectionChanged = false;
                }
                if (hasViewChanged)
                {
                    viewMatrix = Matrix4.LookAt(position_lookAt + camera_pos, position_lookAt, Vector3.UnitY);
                    hasViewChanged = false;
                }

                viewProjectionMatrix = viewMatrix * projectionMatrix;
            }

        }

        public void CastRay(ref Line line)
        {
            Vector4 tempPoints;

            float mouseX = Core.NativeWindow.MouseState.X;
            float mouseY = Core.NativeWindow.MouseState.Y;

            int[] viewport = new int[4];
            OpenTK.Graphics.OpenGL.GL.GetInteger(OpenTK.Graphics.OpenGL.GetPName.Viewport, viewport);

            //ray near
            tempPoints.X = (mouseX - (float)viewport[0]) / (float)viewport[2] * 2.0f - 1.0f;
            tempPoints.Y = 1 - (mouseY - (float)viewport[1]) / (float)viewport[3] * 2.0f;
            tempPoints.Z = -1.0f;
            tempPoints.W = 1.0f;

            tempPoints *= (Matrix4.Invert(projectionMatrix) * Matrix4.Invert(viewMatrix));
            line.min = new Vector3(tempPoints.X, tempPoints.Y, tempPoints.Z) / tempPoints.W;

            //ray far
            tempPoints.X = (mouseX - (float)viewport[0]) / (float)viewport[2] * 2.0f - 1.0f;
            tempPoints.Y = 1 - (mouseY - (float)viewport[1]) / (float)viewport[3] * 2.0f;
            tempPoints.Z = 1;
            tempPoints.W = 1.0f;

            tempPoints *= (Matrix4.Invert(projectionMatrix) * Matrix4.Invert(viewMatrix));
            line.max = new Vector3(tempPoints.X, tempPoints.Y, tempPoints.Z) / tempPoints.W;
        }

        //currently accounts from y 0 
        public void CastRayCorrectedForPlane(ref Line line)
        {
            CastRay(ref line);
            float a = line.min.Y - line.max.Y;
            line.max = line.min + (line.max - line.min) * ((line.max.Y + a) / a);
        }
    }
}
