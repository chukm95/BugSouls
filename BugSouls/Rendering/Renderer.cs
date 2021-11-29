using BugSouls.GamestateManagement;
using BugSouls.ResourceManagement.Shaders;
using BugSouls.Util;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls.Rendering
{
    internal class Renderer
    {
        private Window window;
        private GameStateManager gameStateManager;

        private FrameBufferAttachment gameColorAttachment;
        private FrameBufferAttachment gameDepthAttachment;
        private FrameBuffer gameFrameBuffer;
        private FrameBufferAttachment guiColorAttachment;
        private FrameBufferAttachment guiDepthAttachment;
        private FrameBuffer guiFrameBuffer;

        private Matrix4 projMatrix;
        private Batcher batcher;

        private Shader shader;
        private ShaderUniform su_projectionMatrix;
        private ShaderUniform su_useLight;
        private ShaderUniform su_texture;

        public Renderer()
        {
            window = Core.Window;
            gameStateManager = Core.GameStateManager;

            gameColorAttachment = new FrameBufferAttachment(FramebufferAttachment.ColorAttachment0,
                DrawBuffersEnum.ColorAttachment0,
                PixelInternalFormat.Rgba,
                PixelFormat.Rgba,
                PixelType.UnsignedByte);

            gameDepthAttachment = new FrameBufferAttachment(FramebufferAttachment.DepthAttachment,
               DrawBuffersEnum.None,
               PixelInternalFormat.DepthComponent32f,
               PixelFormat.DepthComponent,
               PixelType.Float);

            gameFrameBuffer = new FrameBuffer(1280, 720, gameColorAttachment, gameDepthAttachment);

            guiColorAttachment = new FrameBufferAttachment(FramebufferAttachment.ColorAttachment0,
                DrawBuffersEnum.ColorAttachment0,
                PixelInternalFormat.Rgba,
                PixelFormat.Rgba,
                PixelType.UnsignedByte);

            guiDepthAttachment = new FrameBufferAttachment(FramebufferAttachment.DepthAttachment,
               DrawBuffersEnum.None,
               PixelInternalFormat.DepthComponent32f,
               PixelFormat.DepthComponent,
               PixelType.Float);

            guiFrameBuffer = new FrameBuffer(1280, 720, guiColorAttachment, guiDepthAttachment);

            Core.Window.OnResize += Window_OnResize;

            projMatrix = Matrix4.CreateOrthographic(1280, 720, 0.1f, 100f);

            batcher = new Batcher(1, Batcher.ShapeType.QUAD, BufferUsageHint.StaticDraw);
            batcher.Begin();
            batcher.Batch(new Vector3(0, 0, -1), Vector3.Zero, new Vector3(1280, 720, 1), new Vector4(0, 0, 1, 1), Color4.White, 0);
            batcher.End();

            shader = Core.ShaderManager.LoadShader("*/Assets/Shaders/BatchingShader.txt");
            su_projectionMatrix = shader["projectionMatrix"];
            su_useLight = shader["useLight"];
            su_texture = shader["sampler"];

            GL.ClearColor(0, 0, 0, 1);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
        }

        private void Window_OnResize(int width, int height)
        {
            gameFrameBuffer.Resize(width, height);
            guiFrameBuffer.Resize(width, height);
        }

        public void Render(TimeSpan deltaTime)
        {
            //render the game
            gameFrameBuffer.Bind();
            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Viewport(0, 0, window.Width, window.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            gameStateManager.CurrentGameState?.RenderGame(deltaTime);

            gameFrameBuffer.Unbind();
            //render the gui
            guiFrameBuffer.Bind();
            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Viewport(0, 0, window.Width, window.Height);
            gameStateManager.CurrentGameState?.RenderGui(deltaTime);
            guiFrameBuffer.Unbind();

            //do post processing

            //draw final
            GL.ClearColor(0f, 0, 0, 1f);
            GL.Viewport(0, 0, window.Width, window.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            shader.Bind();
            su_projectionMatrix.Set(projMatrix);
            su_useLight.Set(false);
            su_texture.Set(0);

            gameColorAttachment.Bind(0);
            batcher.Draw();
        }

        public void CleanUp()
        {
            batcher.Dispose();
        }

    }
}
