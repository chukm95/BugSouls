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

        }


    }
}
