using BugSouls.Rendering;
using BugSouls.ResourceManagement.Shaders;
using BugSouls.ResourceManagement.Textures;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls.GamestateManagement
{
    internal class GS_BufferImageTest : GameState
    {
        private Batcher batcher;

        private Texture testTexture;

        private Shader testShader;        
        private ShaderUniform projectionMatrix;
        private ShaderUniform textureSampler;
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
                batcher.Batch(position, rotation, scale, new Vector4(0, 0, 1, 1), color, 0);
            }
            batcher.End();

            testTexture = textureManager.LoadShader("*/Assets/Textures/Test.png");
            testShader = shaderManager.LoadShader("*/Assets/Shaders/BatchImageTestShader.txt");
            projectionMatrix = testShader["projectionMatrix"];
            textureSampler = testShader["testTexture"];

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
            textureSampler.Set(0);
            testTexture.BindTexture(0);

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
