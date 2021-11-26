using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls.Rendering
{
    internal class TileSheet
    {
        public Vector4 this[int i]
        {
            get
            {
                return frames[i % numOfFrames];
            }
        }

        public int FramesHor
        {
            get => framesHor;
        }

        public int FramesVer
        {
            get => framesVer;
        }

        public int NumOfFrames
        {
            get => numOfFrames;
        }

        private int framesHor;
        private int framesVer;
        private int numOfFrames;
        private Vector4[] frames;

        public TileSheet(int framesHor, int framesVer)
        {
            this.framesHor = framesHor;
            this.framesVer = framesVer;
            numOfFrames = framesHor * framesVer;
            frames = new Vector4[numOfFrames];
            CalculateFrames();
        }

        private void CalculateFrames()
        {
            float x_scale = 1f / (float)framesHor;
            float y_scale = 1f / (float)framesVer;
            int index = 0;

            for(int y = framesVer-1; y >= 0; y--)
            {
                for(int x = 0; x < framesHor; x++)
                {
                    frames[index] = new Vector4(x_scale * x, y_scale * y, x_scale, y_scale);
                    index++;
                }
            }
        }
    }
}
