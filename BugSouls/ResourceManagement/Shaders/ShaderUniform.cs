using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls.ResourceManagement.Shaders
{
    internal class ShaderUniform
    {
        private const int UNIFORM_NAME_BUFF_SIZE = 60;

        public string Name
        {
            get => name;
        }

        private int locationId;
        private int size;
        private ActiveUniformType type;
        private string name;
        private Shader shader;

        internal ShaderUniform(int programId, int locationId, Shader shader)
        {
            int length;
            GL.GetActiveUniform(programId, locationId, UNIFORM_NAME_BUFF_SIZE, out length, out size, out type, out name);
            this.locationId = locationId;
            this.shader = shader;
        }

        public void Set(int value)
        {
            if ((type == ActiveUniformType.Int || type == ActiveUniformType.Sampler2D) && shader.Loaded)
            {
                GL.Uniform1(locationId, value);
            }
        }

        public void Set(int[] values)
        {
            if ((type == ActiveUniformType.Int || type == ActiveUniformType.Sampler2D) && shader.Loaded)
            {
                GL.Uniform1(locationId, values.Length, values);
            }
        }

        public void Set(float value)
        {
            if (type == ActiveUniformType.Float && shader.Loaded)
            {
                GL.Uniform1(locationId, value);
            }
        }

        public void Set(double value)
        {
            if (type == ActiveUniformType.Double && shader.Loaded)
            {
                GL.Uniform1(locationId, value);
            }
        }


        public void Set(bool value)
        {
            if (type == ActiveUniformType.Bool && shader.Loaded)
            {
                GL.Uniform1(locationId, value ? 1 : 0);
            }
        }

        public void Set(Vector2 value)
        {
            if (type == ActiveUniformType.FloatVec2 && shader.Loaded)
            {
                GL.Uniform2(locationId, value);
            }
        }

        public void Set(Vector3 value)
        {
            if (type == ActiveUniformType.FloatVec3 && shader.Loaded)
            {
                GL.Uniform3(locationId, value);
            }
        }


        public void Set(Vector4 value)
        {
            if (type == ActiveUniformType.FloatVec4 && shader.Loaded)
            {
                GL.Uniform4(locationId, value);
            }
        }

        public void Set(Matrix2 value)
        {
            if (type == ActiveUniformType.FloatMat2 && shader.Loaded)
            {
                GL.UniformMatrix2(locationId, false, ref value);
            }
        }

        public void Set(Matrix2 value, bool transposed)
        {
            if (type == ActiveUniformType.FloatMat2 && shader.Loaded)
            {
                GL.UniformMatrix2(locationId, transposed, ref value);
            }
        }

        public void Set(Matrix3 value)
        {
            if (type == ActiveUniformType.FloatMat3 && shader.Loaded)
            {
                GL.UniformMatrix3(locationId, false, ref value);
            }
        }

        public void Set(Matrix3 value, bool transposed)
        {
            if (type == ActiveUniformType.FloatMat3 && shader.Loaded)
            {
                GL.UniformMatrix3(locationId, transposed, ref value);
            }
        }

        public void Set(Matrix4 value)
        {
            if (type == ActiveUniformType.FloatMat4 && shader.Loaded)
            {
                GL.UniformMatrix4(locationId, false, ref value);
            }
        }

        public void Set(Matrix4 value, bool transposed)
        {
            if (type == ActiveUniformType.FloatMat4 && shader.Loaded)
            {
                GL.UniformMatrix4(locationId, transposed, ref value);
            }
        }
    }
}
