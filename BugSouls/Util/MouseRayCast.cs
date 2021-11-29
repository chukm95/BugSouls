using BugSouls.Rendering.Cameras;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls.Util
{
    internal class MouseRayCast
    {
        private static Vector3 rayCast = new Vector3(0, 0, -1);

        public static Vector3 CalculateMouseRay(Camera camera)
        {
            Vector2 normalizedCoords = getNormalisedDeviceCoordinates();
            Vector4 clipCoords = new Vector4(normalizedCoords.X, normalizedCoords.Y, 1.0f, 1.0f);
            return Vector4.TransformRow(clipCoords, Matrix4.Invert(camera.ViewProjection)).Normalized().Xyz;
        }

        private static Vector3 toWorldCoords(Vector4 eyeCoords, Matrix4 viewInverted)
        {
            Vector4 rayWorld = eyeCoords * viewInverted;
            Vector3 mouseRay = new Vector3(rayWorld.X, rayWorld.Y, rayWorld.Z);
            return mouseRay.Normalized();
        }

        private static Vector4 toEyeCoords(Vector4 clipCoords, Matrix4 projectionInverted)
        {
            Vector4 eyeCoords = clipCoords * projectionInverted;
            return new Vector4(eyeCoords.X, eyeCoords.Y, -1f, 0f);
        }

        private static Vector2 getNormalisedDeviceCoordinates()
        {
            Window window = Core.Window;
            float mouseX = Core.NativeWindow.MouseState.X;
            float mouseY = Core.NativeWindow.MouseState.Y;

            float x = (2.0f * mouseX) / window.Width - 1f;
            float y = (2.0f * mouseY) / window.Height - 1f;
            return new Vector2(x, -y);
        }

        public static Vector3 RayPointFar(Camera camera)
        {
            float mouseX = Core.NativeWindow.MouseState.X;
            float mouseY = Core.NativeWindow.MouseState.Y;

            int[] viewport = new int[4];
            OpenTK.Graphics.OpenGL.GL.GetInteger(OpenTK.Graphics.OpenGL.GetPName.Viewport, viewport);

            Vector4 pos = new Vector4();

            // Map x and y from window coordinates, map to range -1 to 1 
            pos.X = (mouseX - (float)viewport[0]) / (float)viewport[2] * 2.0f - 1.0f;
            pos.Y = 1 - (mouseY - (float)viewport[1]) / (float)viewport[3] * 2.0f;
            pos.Z = 1;
            pos.W = 1.0f;

            Vector4 pos2 = pos * (Matrix4.Invert(camera.Projecion) * Matrix4.Invert(camera.View));
            Vector3 pos_out = new Vector3(pos2.X, pos2.Y, pos2.Z);

            return pos_out / pos2.W;
        }

        public static Vector3 RayPointNear(Camera camera)
        {
            Window window = Core.Window;
            float mouseX = Core.NativeWindow.MouseState.X;
            float mouseY = Core.NativeWindow.MouseState.Y;

            int[] viewport = new int[4];
            OpenTK.Graphics.OpenGL.GL.GetInteger(OpenTK.Graphics.OpenGL.GetPName.Viewport, viewport);

            Vector4 pos = new Vector4();

            // Map x and y from window coordinates, map to range -1 to 1 
            pos.X = (mouseX - (float)viewport[0]) / (float)viewport[2] * 2.0f - 1.0f;
            pos.Y = 1 - (mouseY - (float)viewport[1]) / (float)viewport[3] * 2.0f;
            pos.Z = -1.0f;
            pos.W = 1.0f;

            Vector4 pos2 = pos * (Matrix4.Invert(camera.Projecion) * Matrix4.Invert(camera.View));
            Vector3 pos_out = new Vector3(pos2.X, pos2.Y, pos2.Z);

            return pos_out / pos2.W;
        }
    }
}
