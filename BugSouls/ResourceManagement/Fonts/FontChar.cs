using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls.ResourceManagement.Fonts
{
    internal struct FontChar
    {
        public char character;
        public float offsetX;
        public float offsetY;
        public float advance;
        public float width;
        public float height;
        public float top;
        public float bottom;
        public float left;
        public float right;
        public float tex_x;
        public float tex_y;
        public float tex_z;
        public float tex_w;
        public Vector4 texCoords;
        public Vector4 pxCoords;
    }
}
