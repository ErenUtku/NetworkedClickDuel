using GameState;
using UnityEngine;
using Utils.State;

namespace Controllers.State
{
    public class GameStateManager : MonoBehaviour
    {
        public StateMachine StateMachine;
        public GameStates currentGameState;

        public static GameStateManager Instance;
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            StateMachine = new StateMachine();
            StateMachine.Initialize();
        }

        private void Update()
        {
            StateMachine.Update();
        }
    }

    public enum GameStates
    {
        Loading,
        Gameplay,
        Result
    }
}