using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Controllers
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Score")]
        [SerializeField] private int scoreValue;
    
        public static Action<GameTextType,string> ScoreChange;
    
        public int Score
        {
            get => scoreValue;
            private set => scoreValue = value;
        }

        public static PlayerController Instance;
        private void Awake()
        {
            Instance = this;
            
            Score = 0;
        }
    
        public void AddScore()
        {
            Score += 1;
            ScoreChange?.Invoke(GameTextType.Score,Score.ToString());
        
            ExitGames.Client.Photon.Hashtable scoreProps = new ExitGames.Client.Photon.Hashtable();
            scoreProps["Score"] = Score;
            PhotonNetwork.LocalPlayer.SetCustomProperties(scoreProps);
        
            PhotonNetwork.RaiseEvent(1, Score, RaiseEventOptions.Default, SendOptions.SendReliable);
        }
    }
}
