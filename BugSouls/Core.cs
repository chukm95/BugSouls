using BugSouls.Util;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
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

        public static Window Window
        {
            get => instance.window;
        }

        private NativeWindow nativeWindow;
        private bool isRunning;

        private Window window;

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
            //create a native window
            NativeWindowSettings nws = new NativeWindowSettings();
            nws.Title = "Bug Souls";
            nws.Size = new Vector2i(1280, 720);
            nws.WindowBorder = WindowBorder.Resizable;
            nws.WindowState = WindowState.Normal;
            nws.StartFocused = true;
            nws.StartVisible = true;
            nws.Profile = ContextProfile.Core;
            nws.API = ContextAPI.OpenGL;
            nws.APIVersion = new Version(3, 3);

            nativeWindow = new NativeWindow(nws);
            nativeWindow.Context.MakeCurrent();

            //pass native window to the window
            window = new Window(nativeWindow);
            //add the window close request
            window.OnCloseRequested += Quit;

            //final init is telling the game loop we are running
            isRunning = true;
        }

        private void Update()
        {
            //process window events
            nativeWindow.ProcessEvents();
        }

        private void Render()
        {
            //swap frame buffers
            nativeWindow.Context.SwapBuffers();
        }

        private void Deinitialize()
        {
            //close window
            nativeWindow.Close();
            //exit game
            Environment.Exit(0);
        }
    }
}
