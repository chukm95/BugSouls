using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls.ResourceManagement.Fonts
{
    internal class FontManager
    {
        private Dictionary<string, FontMap> fontList;

        public FontManager()
        {
            fontList = new Dictionary<string, FontMap>();
        }

        public FontMap LoadShader(string name, string path, int size)
        {
            if (path.Contains('*'))
            {
                path = path.Replace("*", Directory.GetCurrentDirectory());
                path = path.Replace("/", "\\");
            }


            if (fontList.ContainsKey(name))
            {
                return fontList[name];
            }
            else
            {
                FontMap font = new FontMap(name, path, size);
                if (font.Loaded)
                {
                    fontList.Add(name, font);
                    return font;
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
            foreach (FontMap f in fontList.Values)
            {
                f.Dispose();
            }
            fontList.Clear();
        }
    }
}
