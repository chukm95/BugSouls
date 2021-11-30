using BugSouls.GamestateManagement;
using BugSouls.Rendering;
using BugSouls.ResourceManagement.Shaders;
using BugSouls.ResourceManagement.Textures;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls.GameObjects
{
    internal class Room
    {
        public const int MAX_ROOM_SIZE = 65;
        private const int SEED = 23417;

        private Vector3 rotation_up = new Vector3(MathHelper.DegreesToRadians(-90), 0, 0);
        private Vector3 rotation_north = new Vector3(0, MathHelper.DegreesToRadians(180), 0);
        private Vector3 rotation_east = new Vector3(0, MathHelper.DegreesToRadians(90), 0);
        private Vector3 rotation_south = new Vector3(0, 0, 0);
        private Vector3 rotation_west = new Vector3(0, MathHelper.DegreesToRadians(-90), 0);               

        private Texture roomTexture;

        private byte[,] map;

        private Batcher batcher;

        private TileSheet tileSheet;

        private Random random;

        private Vector3 lastCursorPos;

        private bool hasChanged;

        //room in edit mode
        public Room()
        {
            roomTexture = Core.TextureManager.LoadTexture("*/Assets/Textures/Texture_sec.png");

            map = new byte[MAX_ROOM_SIZE, MAX_ROOM_SIZE];


            map[33, 33] = 1;

            tileSheet = new TileSheet(8, 8);

            batcher = new Batcher(30000, Batcher.ShapeType.QUAD, OpenTK.Graphics.OpenGL.BufferUsageHint.DynamicDraw);

            random = new Random(SEED);

            lastCursorPos = Vector3.Zero;

            hasChanged = true;
        }

        public void Edit(EditMode editMode, int x, int y)
        {
            if(x >= 0 && x < MAX_ROOM_SIZE && y >= 0 && y < MAX_ROOM_SIZE)
            {
                switch(editMode)
                {
                    case EditMode.TILE_REMOVER:
                        map[x, y] = 0;
                        break;
                    case EditMode.TILE_PLACER:
                        map[x, y] = 1;
                        break;
                    case EditMode.TILE_WATER:
                        map[x, y] = 1;
                        break;
                }
            }
        }

        public void Update()
        {
            Random random = new Random(SEED);

            batcher.Begin();
            for (int y = 1; y < MAX_ROOM_SIZE - 1; y++)
            {
                for (int x = 1; x < MAX_ROOM_SIZE - 1; x++)
                {
                    //draw floor
                    batcher.Batch(new Vector3(x * 32, -1, y * 32), rotation_up, new Vector3(32, 32, 1), tileSheet[7], Color4.White, 0);
                    CheckForFloorTile(x, y, random);
                    CheckForWall(x, y, random);
                }
            }
            batcher.End();

            hasChanged = false;
        }

        private void CheckForFloorTile(int x, int y, Random random)
        {
            if (map[x, y] == 1)
            {
                //draw undecorated tile
                batcher.Batch(new Vector3(x * 32, 0, y * 32), rotation_up, new Vector3(32, 32, 1), tileSheet[4], Color4.White, 0);
            }
        }

        private void CheckForWall(int x, int y, Random random)
        {

            if (map[x, y] == 0)
            {
                if (map[x - 1, y] == 1)
                {
                    //wall to left
                    batcher.Batch(new Vector3((x * 32) - 16, 24, y * 32), rotation_west, new Vector3(32, 48, 1), tileSheet[random.Next(0, 3)], Color4.White, 0);
                }
                if (map[x + 1, y] == 1)
                {
                    //wall to right
                    batcher.Batch(new Vector3((x * 32) + 16, 24, y * 32), rotation_east, new Vector3(32, 48, 1), tileSheet[random.Next(0, 3)], Color4.White, 0);
                }
                if (map[x, y - 1] == 1)
                {
                    //wall to forward
                    batcher.Batch(new Vector3(x * 32, 24, (y * 32) -16), rotation_north, new Vector3(32, 48, 1), tileSheet[random.Next(0, 3)], Color4.White, 0);
                }
                if (map[x, y + 1] == 1)
                {
                    //wall above
                    batcher.Batch(new Vector3(x * 32, 24, (y * 32)+16), rotation_south, new Vector3(32, 48, 1), tileSheet[random.Next(0, 3)], Color4.White, 0);
                }
            }
        }


        public void Draw()
        {
            roomTexture.BindTexture(0);
            batcher.Draw();
        }

        public void Dispose()
        {
            batcher.Dispose();
        }

    }
}
