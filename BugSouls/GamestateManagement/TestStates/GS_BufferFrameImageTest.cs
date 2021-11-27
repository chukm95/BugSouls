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

namespace BugSouls.GamestateManagement.TestState
{
    internal class GS_BufferFrameImageTest : GameState
    {
        private Batcher batcher;

        private Texture testTexture;

        private Shader testShader;        
        private ShaderUniform projectionMatrix;
        private ShaderUniform textureSampler;
        private Matrix4 projectionMat;

        protected override void OnInitialize()
        {
            

            TileSheet ts = new TileSheet(2, 2);
            batcher = new Batcher(4, Batcher.ShapeType.QUAD, BufferUsageHint.StaticDraw);
            batcher.Begin();            
            batcher.Batch(new Vector3(-60, 0, -1), new Vector3(0, 0, 0), new Vector3(32, 32, 1), ts[0], Color4.White, 0);
            batcher.Batch(new Vector3(-20, 0, -1), new Vector3(0, 0, 0), new Vector3(32, 32, 1), ts[1], Color4.White, 0);
            batcher.Batch(new Vector3(20, 0, -1), new Vector3(0, 0, 0), new Vector3(32, 32, 1), ts[2], Color4.White, 0);
            batcher.Batch(new Vector3(60, 0, -1), new Vector3(0, 0, 0), new Vector3(32, 32, 1), ts[3], Color4.White, 0);
            batcher.End();

            testTexture = textureManager.LoadShader("*/Assets/Textures/FrameTest.png");
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
