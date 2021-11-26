using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls.ResourceManagement.Shaders
{
    internal class ShaderManager
    {
        private Dictionary<string, Shader> shaderList;

        public ShaderManager()
        {
            shaderList = new Dictionary<string, Shader>();
        }
        
        public Shader LoadShader(string path)
        {
            if(path.Contains('*'))
            {
                path = path.Replace("*", Directory.GetCurrentDirectory());
                path = path.Replace("/", "\\");
            }

            if (shaderList.ContainsKey(path))
            {
                return shaderList[path];
            }
            else
            {
                Shader shader = new Shader(path);
                if (shader.Loaded)
                {
                    shaderList.Add(path, shader);
                    return shader;
                }
                else
                {
                    return null;
                }
            }
        }

        //TODO add shader unload function
        public void CleanUp()
        {
            foreach(Shader s in shaderList.Values)
            {
                s.Delete();
            }
            shaderList.Clear();
        }


    }
}
