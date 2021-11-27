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
    internal class GS_TriangleTest : GameState
    {
        private Vector3[] vertices = new Vector3[]
        {
            new Vector3(0, 32, -1),
            new Vector3(32, -32, -1),
            new Vector3(-32, -32, -1)
        };

        private uint[] indices = new uint[]
        {
            0, 1, 2
        };

        private int vao;
        private int vbo;
        private int ebo;

        private Shader testShader;
        private ShaderUniform projectionMatrix;
        private Matrix4 projectionMat;

        protected override void OnInitialize()
        {
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, Vector3.SizeInBytes * vertices.Length, vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

            ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * indices.Length, indices, BufferUsageHint.StaticDraw);

            testShader = shaderManager.LoadShader("*/Assets/Shaders/TestShader.txt");
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

            GL.BindVertexArray(vao);
            GL.EnableVertexAttribArray(0);

            GL.DrawElements(PrimitiveType.Triangles, 3, DrawElementsType.UnsignedInt, IntPtr.Zero);

            GL.DisableVertexAttribArray(0);

        }

        public override void RenderGui(TimeSpan deltaTime)
        {
            
        }               

        protected override void OnDeinitialize()
        {
            GL.DeleteVertexArray(vao);
            GL.DeleteBuffer(vbo);
            GL.DeleteBuffer(ebo);
        }
        
    }
}
