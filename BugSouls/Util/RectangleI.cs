using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls.Util
{
    internal struct RectangleI
    {
        public Vector2i location;
        public Vector2i size;

        public bool Contains(int x, int y)
        {
            if (x < location.X || x > location.X + size.X)
                return false;
            else if (y < location.Y || y > location.Y + size.Y)
                return false;
            else
                return true;                
        }
    }
}
