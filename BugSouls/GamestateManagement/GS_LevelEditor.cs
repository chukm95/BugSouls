using BugSouls.GameObjects;
using BugSouls.Rendering.Cameras;
using BugSouls.ResourceManagement.Shaders;
using BugSouls.ResourceManagement.Textures;
using BugSouls.Util;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls.GamestateManagement
{
    internal class GS_LevelEditor : GameState
    {
        public enum EditMode
        {
            TILES,
            OBJECTS
        }
               

        private Shader shader;
        private ShaderUniform su_projectionMatrix;
        private ShaderUniform su_useLight;
        private ShaderUniform su_ambient;
        private ShaderUniform su_diffuse_dir;
        private ShaderUniform su_diffuse_col;
        private ShaderUniform su_texture;

        private Line ray;
        private DefaultCamera camera;

        private Room levelEditRoom;

        protected override void OnInitialize()
        {
            shader = shaderManager.LoadShader("*/Assets/Shaders/BatchingShader.txt");
            su_projectionMatrix = shader["projectionMatrix"];
            su_useLight = shader["useLight"];
            su_ambient = shader["ambient"];
            su_diffuse_dir = shader["diffuse_dir"];
            su_diffuse_col = shader["diffuse_color"];
            su_texture = shader["sampler"];

            ray = new Line();
            camera = new DefaultCamera();
            levelEditRoom = new Room();
        }

        public override void Update(TimeSpan deltaTime)
        {
            camera.Update(deltaTime);
            camera.CastRayCorrectedForPlane(ref ray);
            ray.max.X = (int)(((ray.max.X+16) / 32f));
            ray.max.Z = (int)(((ray.max.Z+16) / 32f));
            levelEditRoom.Update(ray.max);           
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
        }

        public override void RenderGui(TimeSpan deltaTime)
        {

        }

        protected override void OnDeinitialize()
        {
            levelEditRoom.Dispose();
        }
    }
}
