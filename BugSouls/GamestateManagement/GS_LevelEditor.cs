using BugSouls.GameObjects;
using BugSouls.Rendering;
using BugSouls.Rendering.Cameras;
using BugSouls.Rendering.Gui;
using BugSouls.ResourceManagement.Shaders;
using BugSouls.ResourceManagement.Textures;
using BugSouls.Util;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls.GamestateManagement
{
    public enum EditMode
    {
        TILE_REMOVER,
        TILE_PLACER,
        TILE_WATER
    }

    internal class GS_LevelEditor : GameState
    {                 
        private Shader shader;
        private ShaderUniform su_projectionMatrix;
        private ShaderUniform su_useLight;
        private ShaderUniform su_ambient;
        private ShaderUniform su_diffuse_dir;
        private ShaderUniform su_diffuse_col;
        private ShaderUniform su_texture;

        private Batcher batcher;

        private bool showButtonList;
        private ButtonList btnList;

        private Line ray;
        private Vector3 cameraPosition;
        private DefaultCamera camera;

        private Vector3 selectionTile;
    
        private EditMode editMode;
        private Room levelEditRoom;

        private bool releaseFirst;
        
        protected override void OnInitialize()
        {
            shader = shaderManager.LoadShader("*/Assets/Shaders/BatchingShader.txt");
            su_projectionMatrix = shader["projectionMatrix"];
            su_useLight = shader["useLight"];
            su_ambient = shader["ambient"];
            su_diffuse_dir = shader["diffuse_dir"];
            su_diffuse_col = shader["diffuse_color"];
            su_texture = shader["sampler"];

            batcher = new Batcher(1, Batcher.ShapeType.QUAD, OpenTK.Graphics.OpenGL.BufferUsageHint.DynamicDraw);

            selectionTile = new Vector3(5000, -1, 5000);

            ray = new Line();
            cameraPosition = new Vector3(33, 0, 33) * 32f;
            camera = new DefaultCamera();
            camera.Update(TimeSpan.Zero);
            levelEditRoom = new Room();
            levelEditRoom.Update();

            releaseFirst = false;

            editMode = EditMode.TILE_PLACER;

            showButtonList = false;
            btnList = new ButtonList();
            btnList.AddAction("Remove Tile", () => { editMode = EditMode.TILE_REMOVER; });
            btnList.AddAction("Set Tile", () => { editMode = EditMode.TILE_PLACER; });
            btnList.AddAction("Set Water", () => { editMode = EditMode.TILE_WATER; });
            btnList.AddAction("test", null);
            btnList.AddAction("test", null);
        }

        public override void Update(TimeSpan deltaTime)
        {
            HandleEditorInput(deltaTime);
            camera.Position = cameraPosition;
            camera.Update(deltaTime);      
        }

        private void HandleEditorInput(TimeSpan deltaTime)
        {
            KeyboardState ks = Core.NativeWindow.KeyboardState;
            MouseState ms = Core.NativeWindow.MouseState;

            //camera movement
            Vector3 movementVec = Vector3.Zero;

            if (camera.RotationDir == DefaultCamera.RotationDirection.NONE)
            {
                if (ks.IsKeyDown(Keys.A))
                {
                    movementVec.X = -128f * (float)deltaTime.TotalSeconds;
                }
                else if (ks.IsKeyDown(Keys.D))
                {
                    movementVec.X = 128f * (float)deltaTime.TotalSeconds;
                }

                if (ks.IsKeyDown(Keys.W))
                {
                    movementVec.Z = -128f * (float)deltaTime.TotalSeconds;
                }
                else if (ks.IsKeyDown(Keys.S))
                {
                    movementVec.Z = 128f * (float)deltaTime.TotalSeconds;
                }

                cameraPosition += Vector3.Transform(movementVec, Quaternion.FromEulerAngles(0, camera.AngleRad, 0));
            }

            //round for pixels
            cameraPosition.X = (float)Math.Round(cameraPosition.X);
            cameraPosition.Z = (float)Math.Round(cameraPosition.Z);


            //check for camera rotation
            if (ks.IsKeyDown(Keys.Left))
                camera.Turn(DefaultCamera.RotationDirection.LEFT);
            else if(ks.IsKeyDown(Keys.Right))
                camera.Turn(DefaultCamera.RotationDirection.RIGHT);

           

            //show buttonList
            if(ks.IsKeyPressed(Keys.F1))
            {
                showButtonList = !showButtonList;
            }

            selectionTile.Y = -1;
            bool menuHover = false;

            //check for buttonlist hover
            if(showButtonList)
            {
                menuHover = btnList.Hover((int)ms.X, (int)ms.Y);                        
            }

            if(!menuHover)
            {
                camera.CastRayCorrectedForPlane(ref ray);
                selectionTile.X = (int)(((ray.max.X + 16) / 32f));
                selectionTile.Z = (int)(((ray.max.Z + 16) / 32f));
                selectionTile.Y = 1;
            }

            if (!ms.IsButtonDown(MouseButton.Button1))
                releaseFirst = false;

            //check for a click
            if (ms.IsButtonDown(MouseButton.Button1) && !ms.WasButtonDown(MouseButton.Button1) && menuHover)
            {
                showButtonList = false;
                btnList.Click();
                releaseFirst = true;

            }
            else if (ms.IsButtonDown(MouseButton.Button1) && !menuHover && !releaseFirst)
            {
                levelEditRoom.Edit(editMode, (int)selectionTile.X, (int)selectionTile.Z);
                levelEditRoom.Update();
            }
        }

        public override void RenderGame(TimeSpan deltaTime)
        {
            shader.Bind();
            su_projectionMatrix.Set(camera.ViewProjection);
            su_useLight.Set(true);
            su_ambient.Set(new Vector3(0.4f, 0.4f, 0.4f));
            su_diffuse_dir.Set(new Vector3(5000, 10000, 10000) * 32f);
            su_diffuse_col.Set(new Vector3(0.6f, 0.6f, 0.6f));
            su_texture.Set(0);
            //drawRoom
            levelEditRoom.Draw();
            //draw selection
            if (selectionTile.Y != -1)
            {
                batcher.Begin();
                batcher.Batch(selectionTile * new Vector3(32, 1, 32), new Vector3(MathHelper.DegreesToRadians(-90), 0, 0), new Vector3(32, 32, 1), Vector4.Zero, Color4.Red, 2);
                batcher.End();
                batcher.Draw();
            }
        }

        public override void RenderGui(TimeSpan deltaTime)
        {
            if(showButtonList)
            {
                btnList.Draw();
            }
        }

        protected override void OnDeinitialize()
        {
            levelEditRoom.Dispose();
            btnList.Dispose();
        }
    }
}
