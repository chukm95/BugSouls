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

        protected GameState()
        {
            isInitialized = false;
            window = Core.Window;
        }

        public void Initialize()
        {
            if(!isInitialized)
            {
                OnInitialize();
                isInitialized = true;
            }
        }

        protected abstract bool OnInitialize();


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
