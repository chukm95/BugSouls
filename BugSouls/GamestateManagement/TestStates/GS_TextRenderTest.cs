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

        private Shader shader;
        private ShaderUniform su_projectionMatrix;
        private ShaderUniform su_useLight;
        private ShaderUniform su_texture;

        private Batcher b;

        private Matrix4 projectionMat;

        protected override void OnInitialize()
        {
            fm = fontManager.LoadFontMap("Outfit_24", "*/Assets/Fonts/Outfit/Outfit-VariableFont_wght.ttf", 24);
            textRenderer = new TextRenderer();
            textRenderer.BufferString(fm, "random string", new Vector2i(32, 1280), Color4.White);

            testShader = shaderManager.LoadShader("*/Assets/Shaders/TextShader.txt");
            projectionMatrix = testShader["projectionMatrix"];
            textureSampler = testShader["testTexture"];

            shader = shaderManager.LoadShader("*/Assets/Shaders/BatchingShader.txt");
            su_projectionMatrix = shader["projectionMatrix"];
            su_useLight = shader["useLight"];
            su_texture = shader["sampler"];

            Vector4 size = fm.MeassureString("random string");

            b = new Batcher(1, Batcher.ShapeType.QUAD | Batcher.ShapeType.CENTERED, BufferUsageHint.StaticDraw);
            b.Begin();
            b.Batch(new Vector3(size.X + 32, size.Y - size.W + 1280, -10), Vector3.Zero, new Vector3(size.Z, size.W, 1), Vector4.Zero, Color4.Red, 2);
            b.End();

            projectionMat = Matrix4.CreateOrthographicOffCenter(0, 1280, 0, 720, -100f, 100f);
            window.OnResize += (w, h) =>
            {
                projectionMat = Matrix4.CreateOrthographicOffCenter(0, w, 0, h, -100f, 100f);
            };
        }

        double time;
        int i;
        public override void Update(TimeSpan deltaTime)
        {
            string s = FontMap.defaultChars + "\n" + FontMap.defaultChars + FontMap.defaultChars;
            time += deltaTime.TotalSeconds;
            
            if(time > 0.025 && i < s.Length)
            {
                time -= 0.025;
                i++;
                textRenderer.BufferString(fm, s.Substring(0, i), new Vector2i(32, 1280), Color4.White);
                Vector4 size = fm.MeassureString(s.Substring(0, i));
                b.Begin();
                b.Batch(new Vector3(size.X + 32, size.Y - size.W + 1280, -10), Vector3.Zero, new Vector3(size.Z, size.W, 1), Vector4.Zero, Color4.Red, 2);
                b.End();
            }
        }

        public override void RenderGame(TimeSpan deltaTime)
        {
            shader.Bind();
            su_projectionMatrix.Set(projectionMat);
            su_useLight.Set(false);
            su_texture.Set(0);
            b.Draw();
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
