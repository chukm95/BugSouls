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
    internal class Batcher
    {
        private int VERTEX_SIZE_IN_BYTES = Vector3.SizeInBytes + Vector3.SizeInBytes + Vector2.SizeInBytes + Vector4.SizeInBytes + sizeof(float);

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Vertex
        {
            public Vector3 position;
            public Vector3 normal;
            public Vector2 uv;
            public Vector4 color;
            public float texChannel;
        }

        [Flags]
        public enum ShapeType : byte
        {
            LINE = 1,
            TRIANGLE = 2,
            QUAD = 4,
            CIRCLE_P16 = 8,
            CIRCLE_P32 = 16, 
            CIRCLE_P64 = 32,
            CIRCLE_P128 = 64,
            CENTERED = 128 
        }



        private Vector3[] templatePositions;
        private Vector3[] templateNormals;
        private Vector2[] templateUvs;
        private uint[] templateIndices;

        private int bufferSize;
        private ShapeType shapeType;
        private BufferUsageHint bufferUsageHint;
        private PrimitiveType primitiveType;

        private Vertex[] vertices;
        private uint[] indices;

        private int vao_id;
        private int vbo_id;
        private int ebo_id;

        private int elementCount;
        private bool isBatching;

        public Batcher(int batchSize, ShapeType shapeType, BufferUsageHint bufferUsageHint)
        {
            this.bufferSize = batchSize;
            this.shapeType = shapeType;
            this.bufferUsageHint = bufferUsageHint;
            primitiveType = PrimitiveType.Triangles;

            //generate the template
            GenerateTemplate();
            //init all the vertices
            vertices = new Vertex[batchSize * templatePositions.Length];
            //init all the indices
            indices = new uint[batchSize * templateIndices.Length];
            //precalculate all the indices
            CalculateAllIndices();
            //init the vertexarray
            InitializeVertexArray();
        }

        private void GenerateTemplate()
        {
            if ((shapeType & ShapeType.LINE) == ShapeType.LINE)
            {
                templatePositions = new Vector3[2];
                templateNormals = new Vector3[2];
                templateUvs = new Vector2[2];
                templateIndices = new uint[]
                {
                    0, 1
                };
                primitiveType = PrimitiveType.Lines;
            }
            else if ((shapeType & ShapeType.TRIANGLE) == ShapeType.TRIANGLE)
            {
                //triangle
                templatePositions = new Vector3[]
                {
                    new Vector3(0, 0.5f, 0),
                    new Vector3(0.5f, -0.5f, 0),
                    new Vector3(-0.5f, -0.5f, 0)
                };
                templateNormals = new Vector3[]
                {
                    new Vector3(0, 0, 1),
                    new Vector3(0, 0, 1),
                    new Vector3(0, 0, 1)
                };
                templateUvs = new Vector2[]
                {
                    new Vector2(0.5f, 1f),
                    new Vector2(1f, 0f),
                    new Vector2(0, 0f)
                };
                templateIndices = new uint[]
                {
                    0, 1, 2
                };
            }
            else if ((shapeType & ShapeType.QUAD) == ShapeType.QUAD)
            {
                //quad
                templatePositions = new Vector3[]
                {
                    new Vector3(-0.5f, 0.5f, 0),
                    new Vector3(0.5f, 0.5f, 0),
                    new Vector3(0.5f, -0.5f, 0),
                    new Vector3(-0.5f, -0.5f, 0)
                };
                templateNormals = new Vector3[]
                {
                    new Vector3(0, 0, 1),
                    new Vector3(0, 0, 1),
                    new Vector3(0, 0, 1),
                    new Vector3(0, 0, 1)
                };
                templateUvs = new Vector2[]
                {
                    new Vector2(0f, 1f),
                    new Vector2(1f, 1f),
                    new Vector2(1f, 0f),
                    new Vector2(0f, 0f)
                };
                templateIndices = new uint[]
                {
                    0, 1, 2,   0, 2, 3
                };
            }
            else if ((shapeType & ShapeType.CIRCLE_P16) == ShapeType.CIRCLE_P16)
            {
                //circle p16
                GenerateCircleTemplate(16);
            }
            else if ((shapeType & ShapeType.CIRCLE_P32) == ShapeType.CIRCLE_P32)
            {
                //circle p32
                GenerateCircleTemplate(32);
            }
            else if ((shapeType & ShapeType.CIRCLE_P64) == ShapeType.CIRCLE_P64)
            {
                //circle p64
                GenerateCircleTemplate(64);
            }
            else if ((shapeType & ShapeType.CIRCLE_P128) == ShapeType.CIRCLE_P128)
            {
                //circle p128
                GenerateCircleTemplate(128);
            }
            //TODO if not centered offset all the vertices by 0.5f
        }

        private void GenerateCircleTemplate(int points)
        {
            //always add one vertex for the centerpoint
            templatePositions = new Vector3[points + 1];
            templateNormals = new Vector3[points + 1];
            templateUvs = new Vector2[points + 1];

            templatePositions[points] = Vector3.Zero;
            templateNormals[points] = Vector3.UnitZ;
            templateUvs[points] = new Vector2(0.5f, 0.5f);

            //get the incremental angle
            double angle = MathHelper.DegreesToRadians(360.0 / points);                    

            //loop through all the vertices in the circle
            for(int i = 0; i < points; i++)
            {
                templatePositions[i] = new Vector3((float)Math.Sin(angle * i),(float) Math.Cos(angle * i), 0);
                templateNormals[i] = Vector3.UnitZ;
                //we need to add 0.5 to get into to 0 to 1 range instead of the -0.5 to 0.5 range
                templateUvs[i] = new Vector2((float)Math.Sin(angle * i), (float)Math.Cos(angle * i)) + templateUvs[points];
            }

            //generate the indices
            //we need 3 indices per "slice"
            templateIndices = new uint[points * 3];

            //loop through every "slice"
            for(int i = 0; i < points; i++)
            {
                int index = i * 3;
                templateIndices[index    ] = (uint)i;
                //mod points because the center index is not to be taken into account
                templateIndices[index + 1] = (uint)((i + 1)%points);
                //always the center vertex
                templateIndices[index + 2] = (uint)points;
            }
        }

        private void CalculateAllIndices()
        {
            //loop through all elements in the 
            for(int i = 0; i < bufferSize; i++)
            {
                int index = i * templateIndices.Length;

                for(int templateIndex = 0; templateIndex < templateIndices.Length; templateIndex++)
                {
                    indices[index + templateIndex] = templateIndices[templateIndex] + (uint)(templatePositions.Length * i);
                }
            }
        }

        private void InitializeVertexArray()
        {
            vao_id = GL.GenVertexArray();
            GL.BindVertexArray(vao_id);

            vbo_id = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_id);
            GL.BufferData(BufferTarget.ArrayBuffer, bufferSize * templatePositions.Length * VERTEX_SIZE_IN_BYTES, vertices, bufferUsageHint);

            //position
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, VERTEX_SIZE_IN_BYTES, 0);
            //normal
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, VERTEX_SIZE_IN_BYTES, 3 * sizeof(float));
            //uvs
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, VERTEX_SIZE_IN_BYTES, 6 * sizeof(float));
            //color
            GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, VERTEX_SIZE_IN_BYTES, 8 * sizeof(float));
            //texchannel
            GL.VertexAttribPointer(4, 1, VertexAttribPointerType.Float, false, VERTEX_SIZE_IN_BYTES, 12 * sizeof(float));

            ebo_id = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo_id);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * indices.Length, indices, bufferUsageHint);
        }

        public void Begin()
        {
            if(isBatching)
            {
                throw new InvalidOperationException("Cannot begin batching when already batching!");
            }

            elementCount = 0;
            isBatching = true;
        }

        public void End()
        {
            if(!isBatching)
            {
                throw new InvalidOperationException("Cannot end batching when it hasnt begon batching!");
            }

            isBatching = false;
            BatchBuffer();
        }

        //test method
        public void BatchQuad(Vector3 position, Vector3 scale)
        {
            if(isBatching && (shapeType & ShapeType.QUAD) == ShapeType.QUAD)
            {
                //get the insertion index
                int insertionIndex = elementCount * templatePositions.Length;
                //loop through all template positions
                for(int i = 0; i < templatePositions.Length; i++)
                {
                    vertices[insertionIndex + i].position = (templatePositions[i] * scale) + position;
                    vertices[insertionIndex + i].normal = templateNormals[i];
                    vertices[insertionIndex + i].uv = templateUvs[i];
                }
                elementCount++;
            }
        }

        //test methode
        public void BatchTriangle(Vector3 position, Vector3 scale)
        {
            if (isBatching && (shapeType & ShapeType.TRIANGLE) == ShapeType.TRIANGLE)
            {
                //get the insertion index
                int insertionIndex = elementCount * templatePositions.Length;
                //loop through all template positions
                for (int i = 0; i < templatePositions.Length; i++)
                {
                    vertices[insertionIndex + i].position = (templatePositions[i] * scale) + position;
                    vertices[insertionIndex + i].normal = templateNormals[i];
                    vertices[insertionIndex + i].uv = templateUvs[i];
                }
                elementCount++;
            }
        }

        //test methode
        public void BatchLine(Vector3 position_start, Vector3 position_end)
        {
            if (isBatching && (shapeType & ShapeType.LINE) == ShapeType.LINE)
            {
                //get the insertion index
                int insertionIndex = elementCount * templatePositions.Length;
                vertices[insertionIndex].position = position_start;
                vertices[insertionIndex + 1].position = position_end;

                elementCount++;
            }
        }

        public void Batch(Vector3 position, Vector3 rotation, Vector3 scale, Vector4 uv, Color4 color, float texChannel)
        {
            if (isBatching)
            {
                //rotation quat only once
                Quaternion rotationQuat = Quaternion.FromEulerAngles(rotation.X, rotation.Y, rotation.Z);

                //get the insertion index
                int insertionIndex = elementCount * templatePositions.Length;

                //loop through all template positions
                for (int i = 0; i < templatePositions.Length; i++)
                {
                    vertices[insertionIndex + i].position = Vector3.Transform(templatePositions[i] * scale, rotationQuat) + position; 
                    vertices[insertionIndex + i].normal = templateNormals[i];
                    vertices[insertionIndex + i].uv = uv.Xy + (templateUvs[i] * uv.Zw);
                    vertices[insertionIndex + i].color = new Vector4(color.R, color.G, color.B, color.A) / 255f;
                    vertices[insertionIndex + i].texChannel = texChannel;
                }
                elementCount++;
            }
        }

        public void Batch(Matrix4 transform, Vector4 uv, Color4 color, float texChannel)
        {
            if (isBatching)
            {
                //get the insertion index
                int insertionIndex = elementCount * templatePositions.Length;
                //loop through all template positions
                for (int i = 0; i < templatePositions.Length; i++)
                {
                    vertices[insertionIndex + i].position = Matrix4. //Vector3.TransformVector(templatePositions[i], transform);
                    Console.WriteLine(vertices[insertionIndex + i].position.ToString());
                    vertices[insertionIndex + i].normal = templateNormals[i];
                    vertices[insertionIndex + i].uv = uv.Xy + (templateUvs[i] * uv.Zw);
                    vertices[insertionIndex + i].color = new Vector4(color.R, color.G, color.B, color.A);
                    vertices[insertionIndex + i].texChannel = texChannel;
                }
                elementCount++;
            }
        }

        public void BatchLine(Vector3 startPosition, Vector4 endPosition, Color4 color)
        {

        }

        private void BatchBuffer()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_id);
            GL.BufferData(BufferTarget.ArrayBuffer, bufferSize * templatePositions.Length * VERTEX_SIZE_IN_BYTES, vertices, bufferUsageHint);
        }

        public void Draw()
        {
            GL.BindVertexArray(vao_id);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);

            GL.DrawElements(primitiveType, elementCount * templateIndices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);
            GL.DisableVertexAttribArray(3);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(vbo_id);
            GL.DeleteBuffer(ebo_id);
            GL.DeleteVertexArray(vao_id);
        }
    }
}
