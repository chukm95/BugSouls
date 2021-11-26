using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls.ResourceManagement.Textures
{
    internal class TextureManager
    {

        private Dictionary<string, Texture> textureList;

        public TextureManager()
        {
            textureList = new Dictionary<string, Texture>();
        }

        public Texture LoadShader(string path)
        {
            if (path.Contains('*'))
            {
                path = path.Replace("*", Directory.GetCurrentDirectory());
                path = path.Replace("/", "\\");
            }

            if (textureList.ContainsKey(path))
            {
                return textureList[path];
            }
            else
            {
                Texture texture = new Texture(path);
                if (texture.Loaded)
                {
                    textureList.Add(path, texture);
                    return texture;
                }
                else
                {
                    return null;
                }
            }
        }

        public void CleanUp()
        {
            foreach(Texture texture in textureList.Values)
            {
                texture.Dispose();
            }
            textureList.Clear();
        }
    }
}
