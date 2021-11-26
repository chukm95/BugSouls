using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls.ResourceManagement.Shaders
{
    internal class Shader
    {
        private const string VERTEX_TAG = "$VERTEX";
        private const string FRAGMENT_TAG = "$FRAGMENT";

        public string Path
        {
            get => path;
        }

        public bool Loaded
        {
            get => loaded;
        }

        public ShaderUniform this[string name]
        {
            get
            {
                if (shaderUniforms.ContainsKey(name))
                    return shaderUniforms[name];
                else
                    return null;
            }
        }

        private string path;
        private bool loaded;

        private Dictionary<string, ShaderUniform> shaderUniforms;

        private StringBuilder vertexShaderSource;        
        private StringBuilder fragmentShaderSource;
        private int program;

        public Shader(string path)
        {
            this.path = path;
            program = -1;
            shaderUniforms = new Dictionary<string, ShaderUniform>();
            loaded = Load();
            if(!loaded)
                program = -1;
        }

        private bool Load()
        {
            if (File.Exists(path))
            {
                vertexShaderSource = new StringBuilder();
                fragmentShaderSource = new StringBuilder();

                ReadShaderSources();

                if (vertexShaderSource == null && fragmentShaderSource == null)
                {
                    Console.WriteLine("Incomplete shader!");
                    return false;
                }

                int vertexShader = -1, fragmentShader = -1;

                if (!CreateShader(out vertexShader, vertexShaderSource.ToString(), ShaderType.VertexShader) ||
                    !CreateShader(out fragmentShader, fragmentShaderSource.ToString(), ShaderType.FragmentShader))
                {
                    vertexShaderSource.Clear();
                    fragmentShaderSource.Clear();
                    GL.DeleteShader(vertexShader);
                    GL.DeleteShader(fragmentShader);
                    return false;
                }

                //create the program
                program = GL.CreateProgram();

                //create the shaders
                GL.AttachShader(program, vertexShader);
                GL.AttachShader(program, fragmentShader);

                //link the shader
                GL.LinkProgram(program);

                //verify link status
                int linkstatus;
                GL.GetProgram(program, GetProgramParameterName.LinkStatus, out linkstatus);

                if (linkstatus == 0)
                {
                    Console.WriteLine($"Failed to link shaders for {path}!");
                    vertexShaderSource.Clear();
                    fragmentShaderSource.Clear();
                    GL.DeleteShader(vertexShader);
                    GL.DeleteShader(fragmentShader);
                    GL.DeleteProgram(program);

                    return false;
                }

                //check the validation status
                int validationStatus;
                GL.GetProgram(program, GetProgramParameterName.ValidateStatus, out validationStatus);

                if (validationStatus == 0)
                {
                    Console.WriteLine($"Failed to valdiate shaders for {path}!");
                    vertexShaderSource.Clear();
                    fragmentShaderSource.Clear();
                    GL.DeleteShader(vertexShader);
                    GL.DeleteShader(fragmentShader);
                    GL.DeleteProgram(program);

                    return false;
                }

                vertexShaderSource.Clear();
                fragmentShaderSource.Clear();
                GL.DeleteShader(vertexShader);
                GL.DeleteShader(fragmentShader);

                Console.WriteLine($"Succesfully loaded shader {path}!");
                InitShaderUniforms();
                return true;
            }
            return false;
        }

        private void ReadShaderSources()
        {
            using (StreamReader sr = new StreamReader(File.OpenRead(path)))
            {
                string line;
                StringBuilder currentSource = null;
                while((line = sr.ReadLine())!=null)
                {
                    switch(line)
                    {
                        case VERTEX_TAG:
                            if(vertexShaderSource == null)
                                vertexShaderSource = new StringBuilder();
                            currentSource = vertexShaderSource;
                            break;
                        case FRAGMENT_TAG:
                            if(fragmentShaderSource == null)
                                fragmentShaderSource = new StringBuilder();
                            currentSource = fragmentShaderSource;
                            break;
                        default:
                            currentSource.AppendLine(line);
                            break;
                    }
                }
            }
        }

        private bool CreateShader(out int shaderId, string shaderSource, ShaderType shaderType)
        {
            int shaderIdTemp = GL.CreateShader(shaderType);

            //check if we have a valid id
            if (shaderIdTemp == 0)
            {
                //error
                Console.WriteLine($"Cannot create {shaderType.ToString()} shader for {path}!");
                shaderId = -1;
                return false;
            }

            GL.ShaderSource(shaderIdTemp, shaderSource);
            GL.CompileShader(shaderIdTemp);
            int compileStatus;
            GL.GetShader(shaderIdTemp, ShaderParameter.CompileStatus, out compileStatus);

            if (compileStatus == 0)
            {
                Console.WriteLine($"Cannot compile {shaderType.ToString()} shader for {path}!");                
                Console.WriteLine(GL.GetShaderInfoLog(shaderIdTemp));
                GL.DeleteShader(shaderIdTemp);
                shaderId = -1;
                return false;
            }

            shaderId = shaderIdTemp;
            return true;
        }

        private void InitShaderUniforms()
        {
            //get all shaderuniforms
            int numOfUniforms;
            GL.GetProgram(program, GetProgramParameterName.ActiveUniforms, out numOfUniforms);
            //loop through
            for (int i = 0; i < numOfUniforms; i++)
            {
                ShaderUniform su = new ShaderUniform(program, i, this);
                shaderUniforms.Add(su.Name, su);
            }
        }

        public void Bind()
        {
            if(loaded)
                GL.UseProgram(program);
        }

        public void Delete()
        {
            GL.DeleteProgram(program);
        }
    }
}
