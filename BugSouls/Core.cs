using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls
{
    public class Core
    {
        private static Core instance;

        public static void Run()
        {
            if (instance == null)
            {
                instance = new Core();
                instance.GameLoop();
            }
        }

        public static void Quit()
        {
            if (instance != null)
                instance.isRunning = false;
        }

        private bool isRunning;

        private Core()
        {
            instance = this;
        }

        private void GameLoop()
        {
            Initialize();
            while(isRunning)
            {
                Update();
                Render();
            }
            Deinitialize();
        }

        private void Initialize()
        {

            //final init is telling the game loop we are running
            isRunning = true;
        }

        private void Update()
        {

        }

        private void Render()
        {

        }

        private void Deinitialize()
        {

        }
    }
}
