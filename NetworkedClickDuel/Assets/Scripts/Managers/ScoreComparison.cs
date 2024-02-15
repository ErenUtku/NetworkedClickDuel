using Controllers;
using GameState;
using Photon.Pun;
using UnityEngine;

namespace Managers
{
    public class ScoreComparison : MonoBehaviour
    {
        public int Player1Score { get; private set; } = 0;

        public int Player2Score { get; private set; } = 0;

        public static ScoreComparison Instance;
        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            GameplayState.GameIsDone += GameIsDone;
        }

        private void OnDisable()
        {
            GameplayState.GameIsDone -= GameIsDone;
        }

        private void GameIsDone()
        {
            Player1Score = PlayerController.Instance.Score;
        
            Player2Score = (int)PhotonNetwork.PlayerListOthers[0].CustomProperties["Score"];
        
            Debug.Log($"Scores Are P1: {Player1Score} vs P2 : {Player2Score} ");
        }

    }
}