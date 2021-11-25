using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugSouls.GamestateManagement
{
    internal class GameStateManager
    {
        public GameState CurrentGameState
        {
            get => currentGameState;
        }

        private Type currentGameStateType;
        private GameState currentGameState;
        private Dictionary<Type, GameState> gameStateList;

        public GameStateManager()
        {
            currentGameStateType = null;
            currentGameState = null;
            gameStateList = new Dictionary<Type, GameState>();
        }

        public void SetGameState<T>()
        {
            if(currentGameStateType != typeof(T) && gameStateList.ContainsKey(typeof(T)))
            {
                //deinit the old gamestate
                currentGameState?.Deinitialize();

                currentGameStateType = typeof(T);
                currentGameState = gameStateList[typeof(T)];

                //activate new game state
                currentGameState.Initialize();
            }
        }

        public T GetGameState<T>() where T : GameState
        {
            if(gameStateList.ContainsKey(typeof(T)))
            {
                return gameStateList[typeof(T)] as T;
            }
            return null;
        }

        public void AddGameState<T>(T gameState) where T : GameState
        {
            if(!gameStateList.ContainsKey(typeof(T)))
            {
                gameStateList.Add(typeof(T), gameState);
            }
        }

    }
}
