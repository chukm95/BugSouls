using BugSouls.Rendering.Cameras;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls.Util
{
    internal class CameraController
    {
        public enum Rotation
        {
            NONE,
            LEFT,
            RIGHT
        }

        public Vector3 Forward
        {
            get => forward;
        }

        private Camera camera;

        private float speed;

        private Vector3 forward;

        private Vector3 velocity;

        private Rotation currentRotation;
        private float rotation;

        public CameraController(Camera camera, float speed)
        {
            this.camera = camera;
            this.speed = speed;
            currentRotation = Rotation.NONE;
            rotation = 0;
        }        

        public void Update(double deltaTime)
        {
            KeyboardState kbs = Core.NativeWindow.KeyboardState;
            MouseState ms = Core.NativeWindow.MouseState;

            Vector3 speedVec = Vector3.Zero;

            if (currentRotation == Rotation.NONE)
            {

            }
            else
            {
                if (rotation < 180f)
                {
                    rotation += 180f * (float)deltaTime;
                }
            }

            Quaternion rotationQuat = Quaternion.FromEulerAngles(camera.Rotation.X, 0, 0).Inverted();
            speedVec = Vector3.Transform(speedVec, rotationQuat);

            if (kbs.IsKeyDown(Keys.A) && !kbs.IsKeyDown(Keys.D))
                speedVec.X = -1f * (float)deltaTime;
            else if (!kbs.IsKeyDown(Keys.A) && kbs.IsKeyDown(Keys.D))
                speedVec.X = 1f * (float)deltaTime;

            if (kbs.IsKeyDown(Keys.W) && !kbs.IsKeyDown(Keys.S))
                speedVec.Z = -1f * (float)deltaTime;
            else if (!kbs.IsKeyDown(Keys.W) && kbs.IsKeyDown(Keys.S))
                speedVec.Z = 1f * (float)deltaTime;

            rotationQuat = Quaternion.FromEulerAngles(0f, camera.Rotation.Y, camera.Rotation.Z).Inverted();
            speedVec = Vector3.Transform(speedVec, rotationQuat);

            velocity = speedVec * speed;

            forward = Vector3.Transform(Vector3.UnitZ, rotationQuat);
            camera.Position += velocity;
        }

    }
}
