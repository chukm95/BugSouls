using BugSouls.ResourceManagement.Fonts;
using BugSouls.ResourceManagement.Shaders;
using BugSouls.Util;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls.Rendering.Gui
{
    internal class ButtonList
    {
        public struct Button
        {
            public string _name;
            public Action _action;
        }

        private Shader textShader;
        private ShaderUniform su_ts_projectionMatrix;
        private ShaderUniform su_ts_textureSampler;

        private Shader batchShader;
        private ShaderUniform su_bs_projectionMatrix;
        private ShaderUniform su_bs_useLight;
        private ShaderUniform su_bs_texture;

        private FontMap fontMap;
        private TextRenderer textRenderer;
        private Batcher batcher;

        private Matrix4 projectionMatrix;
        private RectangleI bounds;
        private List<Button> actionMap;
        private int maxWidth;
        private bool listDirty;
        private string listString;
        private int offset;

        public ButtonList()
        {
            textShader = Core.ShaderManager.LoadShader("*/Assets/Shaders/TextShader.txt");
            su_ts_projectionMatrix = textShader["projectionMatrix"];
            su_ts_textureSampler = textShader["testTexture"];

            batchShader = Core.ShaderManager.LoadShader("*/Assets/Shaders/BatchingShader.txt");
            su_bs_projectionMatrix = batchShader["projectionMatrix"];
            su_bs_useLight = batchShader["useLight"];
            su_bs_texture = batchShader["sampler"];

            fontMap = Core.FontManager.LoadFontMap("Outfit_24", "*/Assets/Fonts/Outfit/Outfit-VariableFont_wght.ttf", 24);


            textRenderer = new TextRenderer();
            batcher = new Batcher(2, Batcher.ShapeType.QUAD | Batcher.ShapeType.CENTERED, BufferUsageHint.StreamDraw);

            bounds = new RectangleI();
            bounds.location = Vector2i.Zero;
            bounds.size.Y = Core.Window.Height;

            projectionMatrix = Matrix4.CreateOrthographicOffCenter(0, Core.Window.Width, 0, Core.Window.Height, 0, 100);
            Core.Window.OnResize += Window_OnResize;
            actionMap = new List<Button>();
            maxWidth = 0;
            listString = "";
        }

        public void AddAction(string name, Action action)
        {
            actionMap.Add(new Button() { _name = name, _action = action });
            int width = (int)Math.Ceiling(fontMap.MeassureString(name).Z + fontMap.Padding) + 32;
            if (width > maxWidth)
            {
                maxWidth = width;
                bounds.size.X = maxWidth;
            }
            listDirty = true;
            
        }

        private void Window_OnResize(int width, int height)
        {
            projectionMatrix = Matrix4.CreateOrthographicOffCenter(0, width, 0, height, -1, 100);
            bounds.location = Vector2i.Zero;
            bounds.size.X = maxWidth;
            bounds.size.Y = height;
        }

        private void ConcatList()
        {
            listString = "";

            foreach (Button button in actionMap)
            {
                listString = String.Concat(listString, "\n", button._name);
            }

            listDirty = false;
        }

        public bool Hover(int x, int y)
        {
            bool hover = bounds.Contains(x, y);

            if (hover)
                offset = y / (fontMap.Size + 2);
            else
                offset = -1;

            return hover;
        }        

        public void Click()
        {
            if(offset != -1 && offset < actionMap.Count)
                actionMap[offset]._action?.Invoke();
        }

        public void Draw()
        {
            if (listDirty)
                ConcatList();

            batcher.Begin();
            batcher.Batch(Vector3.Zero, Vector3.Zero, new Vector3(maxWidth, Core.Window.Height, 1), Vector4.Zero, new Color4(0.2f, 0.2f, 0.2f, 1f), 2);
            if (offset != -1) {
                batcher.Batch(new Vector3(0, Core.Window.Height - fontMap.Size - (fontMap.Padding * 4) - (offset * (fontMap.Size + fontMap.Padding)), 0), Vector3.Zero, new Vector3(maxWidth, fontMap.Size + fontMap.Padding, 1), Vector4.Zero, new Color4(0.4f, 0.4f, 0.4f, 1f), 2);
            }
            batcher.End();
            batchShader.Bind();
            su_bs_projectionMatrix.Set(projectionMatrix);
            su_bs_useLight.Set(false);
            su_bs_texture.Set(0);
            batcher.Draw();

            textShader.Bind();
            su_ts_projectionMatrix.Set(projectionMatrix);
            su_ts_textureSampler.Set(0);
            textRenderer.BufferString(fontMap, listString, new Vector2i(17, Core.Window.Height), Color4.White);
            textRenderer.Draw();
        }

        public void Dispose()
        {
            Core.Window.OnResize -= Window_OnResize;
            textRenderer.Dispose();
            batcher.Dispose();
        }
    }
}
