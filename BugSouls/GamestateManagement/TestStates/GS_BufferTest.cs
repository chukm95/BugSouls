using BugSouls.Rendering;
using BugSouls.ResourceManagement.Shaders;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls.GamestateManagement.TestState
{
    internal class GS_BufferTest : GameState
    {
        private Batcher batcher;

        private Shader testShader;
        private ShaderUniform projectionMatrix;
        private Matrix4 projectionMat;

        protected override void OnInitialize()
        {
            Random r = new Random();

            batcher = new Batcher(40, Batcher.ShapeType.QUAD, BufferUsageHint.StaticDraw);
            batcher.Begin();
            for(int i = 0; i < 40; i++)
            {
                Vector3 position = new Vector3(r.Next(-640, 640), r.Next(-360, 360), -1);
                Vector3 rotation = new Vector3(0, 0, MathHelper.DegreesToRadians(r.Next(360)));
                Vector3 scale = new Vector3(16 + r.Next(64), 16 + r.Next(64), 1);
                Color4 color = new Color4(64 + r.Next(128), 64 + r.Next(128), 64 + r.Next(128), 255);
                Matrix4 transformMatrix = Matrix4.CreateTranslation(position) * Matrix4.CreateRotationZ(rotation.Z) * Matrix4.CreateScale(scale);
                //i hate myself
                //batcher.Batch(transformMatrix, Vector4.Zero, color, 0);
                batcher.Batch(position, rotation, scale, Vector4.Zero, color, 0);
                //batcher.Batch(new Vector3(0, 0, -1), Vector3.Zero, new Vector3(32, 32, 1), Vector4.Zero, color, 0);
                //batcher.BatchQuad(new Vector3(0, 0, -1), new Vector3(32, 32, 1));
            }
            batcher.End();

            testShader = shaderManager.LoadShader("*/Assets/Shaders/BatchTestShader.txt");
            projectionMatrix = testShader["projectionMatrix"];

            projectionMat = Matrix4.CreateOrthographic(1280, 720, 0.1f, 100f);

            GL.ClearColor(0, 0, 0, 0);

        }

        public override void Update(TimeSpan deltaTime)
        {

        }

        public override void RenderGame(TimeSpan deltaTime)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            testShader.Bind();
            projectionMatrix.Set(projectionMat);

            batcher.Draw();
        }

        public override void RenderGui(TimeSpan deltaTime)
        {
            
        }               

        protected override void OnDeinitialize()
        {
            batcher.Dispose();
        }
        
    }
}
