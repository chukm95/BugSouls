using BugSouls.ResourceManagement.Fonts;
using BugSouls.ResourceManagement.Shaders;
using BugSouls.ResourceManagement.Textures;
using BugSouls.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls.GamestateManagement
{
    internal abstract class GameState
    {
        private bool isInitialized;
        protected readonly Window window;
        protected readonly GameStateManager gameStateManager;
        protected readonly ShaderManager shaderManager;
        protected readonly TextureManager textureManager;
        protected readonly FontManager fontManager;

        protected GameState()
        {
            isInitialized = false;
            window = Core.Window;
            gameStateManager = Core.GameStateManager;
            shaderManager = Core.ShaderManager;
            textureManager = Core.TextureManager;
            fontManager = Core.FontManager;
        }

        public void Initialize()
        {
            if(!isInitialized)
            {
                OnInitialize();
                isInitialized = true;
            }
        }

        protected abstract void OnInitialize();

        public abstract void Update(TimeSpan deltaTime);

        public abstract void RenderGame(TimeSpan deltaTime);

        public abstract void RenderGui(TimeSpan deltaTime);

        public void Deinitialize()
        {
            if(isInitialized)
            {
                OnDeinitialize();
                isInitialized = false;
            }
        }

        protected abstract void OnDeinitialize();
    }
}
