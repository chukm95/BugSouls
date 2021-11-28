using BugSouls.Rendering;
using BugSouls.ResourceManagement.Fonts;
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
    internal class GS_TextRenderTest : GameState
    {
        private FontMap fm;
        private TextRenderer textRenderer;

        private Shader testShader;        
        private ShaderUniform projectionMatrix;
        private ShaderUniform textureSampler;
        private Matrix4 projectionMat;

        protected override void OnInitialize()
        {
            fm = fontManager.LoadFontMap("Outfit_24", "*/Assets/Fonts/Outfit/Outfit-VariableFont_wght.ttf", 24);
            textRenderer = new TextRenderer();
            textRenderer.BufferString(fm, "Test\ntest", new Vector2i(-600, 32), Color4.White);

            testShader = shaderManager.LoadShader("*/Assets/Shaders/TextTestShader.txt");
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

            textRenderer.Draw();
        }

        public override void RenderGui(TimeSpan deltaTime)
        {
            
        }               

        protected override void OnDeinitialize()
        {
            textRenderer.Dispose();
        }
        
    }
}
