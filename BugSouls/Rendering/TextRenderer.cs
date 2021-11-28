using BugSouls.ResourceManagement.Fonts;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls.Rendering
{
    internal class TextRenderer
    {
        public readonly int MAX_STRING_SIZE = 1000;
        public readonly int INDICES_PER_QUAD = 6;
        public readonly int VERTICES_PER_QUAD = 4;
        public readonly int VERTEX_SIZE_IN_BYTES = Vector3.SizeInBytes + Vector2.SizeInBytes + Vector4.SizeInBytes;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Vertex
        {
            public Vector3 position;            
            public Vector2 uv;
            public Vector4 color;            
        }

        private Vector3[] quadVertices = new Vector3[]
        {
            new Vector3(0, 0, -1),
            new Vector3(1, 0, -1),
            new Vector3(1, 1, -1),
            new Vector3(0, 1, -1)
        };

        private Vector2[] quadUvs = new Vector2[]
        {
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, 0)
        };

        private ushort[] quadIndices = new ushort[]
        {
            0, 1, 2, 0, 2, 3
        };

        private int vao_id;
        private int vbo_id;
        private int ebo_id;

        private Vertex[] vertices;
        private ushort[] indices;

        private FontMap fontMap;
        private int numOfChars;

        public TextRenderer()
        {
            vertices = new Vertex[VERTICES_PER_QUAD * MAX_STRING_SIZE];
            indices = new ushort[INDICES_PER_QUAD * MAX_STRING_SIZE];
            numOfChars = 0;

            InitializeIndices();
            InitializeVao();
        }

        private void InitializeIndices()
        {
            for (int i = 0; i < MAX_STRING_SIZE; i++)
            {
                int index = i * INDICES_PER_QUAD;

                for (int templateIndex = 0; templateIndex < INDICES_PER_QUAD; templateIndex++)
                {
                    indices[index + templateIndex] = (ushort)(quadIndices[templateIndex] + (VERTICES_PER_QUAD * i));                    
                }
            }
        }

        private void InitializeVao()
        {
            vao_id = GL.GenVertexArray();
            GL.BindVertexArray(vao_id);

            vbo_id = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_id);
            GL.BufferData(BufferTarget.ArrayBuffer, MAX_STRING_SIZE * VERTICES_PER_QUAD * VERTEX_SIZE_IN_BYTES, vertices, BufferUsageHint.DynamicDraw);

            //position
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, VERTEX_SIZE_IN_BYTES, 0);
            //uvs
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, VERTEX_SIZE_IN_BYTES, 3 * sizeof(float));
            //color
            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, VERTEX_SIZE_IN_BYTES, 5 * sizeof(float));

            ebo_id = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo_id);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(ushort) * indices.Length, indices, BufferUsageHint.DynamicDraw);
        }

        public void BufferString(FontMap fontMap, string text, Vector2i position, Color4 color)
        {
            if (text.Length > MAX_STRING_SIZE)
                throw new Exception("maximum of 1000 chars!");

            this.fontMap = fontMap;
            numOfChars = text.Length;

            int offset = 0;

            for(int i = 0; i < numOfChars; i++)
            {
                FontChar fc = fontMap[text[i]];
                if (text[i] != '\n')
                {
                    int insertionIndex = i * VERTICES_PER_QUAD;
                    for (int v = 0; v < VERTICES_PER_QUAD; v++)
                    {
                        vertices[insertionIndex + v].position = quadVertices[v] * new Vector3(fc.pxCoords.Z, fc.pxCoords.W, 1);
                        vertices[insertionIndex + v].position.X += position.X + offset + fc.offsetX;
                        vertices[insertionIndex + v].position.Y += position.Y - fc.bottom;

                        vertices[insertionIndex + v].uv = fc.texCoords.Xy + (quadUvs[v] * fc.texCoords.Zw);
                        vertices[insertionIndex + v].color = new Vector4(color.R, color.G, color.B, color.A);                        
                    }
                    offset += (int)fc.advance;
                }
                else
                {
                    position.Y -= fontMap.Size + fontMap.Padding;
                    offset = 0;
                }
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_id);
            GL.BufferData(BufferTarget.ArrayBuffer, MAX_STRING_SIZE * VERTICES_PER_QUAD * VERTEX_SIZE_IN_BYTES, vertices, BufferUsageHint.DynamicDraw);
        }

        public void Draw()
        {
            if (fontMap == null || numOfChars < 1)
                return;

            fontMap.BindTexture(0);
            GL.BindVertexArray(vao_id);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);

            GL.DrawElements(PrimitiveType.Triangles, numOfChars * INDICES_PER_QUAD, DrawElementsType.UnsignedShort, IntPtr.Zero);

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(vbo_id);
            GL.DeleteBuffer(ebo_id);
            GL.DeleteVertexArray(vao_id);
        }

    }
}
