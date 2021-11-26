using BugSouls.GamestateManagement;
using BugSouls.ResourceManagement.Shaders;
using BugSouls.ResourceManagement.Textures;
using BugSouls.Util;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls
{
    internal class Core
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

        public static GameStateManager GameStateManager
        {
            get => instance.gameStateManager;
        }

        public static ShaderManager ShaderManager
        {
            get => instance.shaderManager;
        }

        public static TextureManager TextureManager
        {
            get => instance.textureManager;
        }

        private NativeWindow nativeWindow;
        private Stopwatch deltaTimer;
        private TimeSpan deltaTime;
        private bool isRunning;

        private Window window;

        private GameStateManager gameStateManager;
        private ShaderManager shaderManager;
        private TextureManager textureManager;

        private Core()
        {
            instance = this;
        }

        private void GameLoop()
        {
            Initialize();
            while(isRunning)
            {
                //count the deltatime
                deltaTime = deltaTimer.Elapsed;
                deltaTimer.Restart();
                //process window events
                nativeWindow.ProcessEvents();
                //update the currentgamestate
                gameStateManager.CurrentGameState?.Update(deltaTime);
                //render the game
                gameStateManager.CurrentGameState?.RenderGame(deltaTime);
                //render the gui
                gameStateManager.CurrentGameState?.RenderGui(deltaTime);
                //swap frame buffers
                nativeWindow.Context.SwapBuffers();
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

            //create the deltatimer
            deltaTimer = new Stopwatch();
            deltaTimer.Start();

            //pass native window to the window
            window = new Window(nativeWindow);
            //add the window close request
            window.OnCloseRequested += Quit;

            //create the gamestate manager
            gameStateManager = new GameStateManager();
            //create the shader manager
            shaderManager = new ShaderManager();
            //create the texture manager
            textureManager = new TextureManager();

            //add gamestates
            gameStateManager.AddGameState<GS_TriangleTest>(new GS_TriangleTest());
            gameStateManager.AddGameState<GS_BufferTest>(new GS_BufferTest());
            gameStateManager.AddGameState<GS_BufferImageTest>(new GS_BufferImageTest());
            gameStateManager.AddGameState<GS_BufferFrameImageTest>(new GS_BufferFrameImageTest());
            gameStateManager.SetGameState<GS_BufferFrameImageTest>();

            //final init is telling the game loop we are running
            isRunning = true;
        }

        private void Deinitialize()
        {
            //close window
            nativeWindow.Close();
            //cleanup all shaders
            shaderManager.CleanUp();
            //clean up textures
            textureManager.CleanUp();
            //exit game
            Environment.Exit(0);
        }
    }
}
