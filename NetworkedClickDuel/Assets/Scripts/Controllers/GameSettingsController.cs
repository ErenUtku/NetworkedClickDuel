using UnityEngine;

namespace Controllers
{
    public class GameSettingsController : MonoBehaviour
    {
    
        [Header("Game Settings")]
        [SerializeField] private float countDownTimer;
        [SerializeField] private float gameplayTimer;
    
        public float CountDownTimer { get; private set; }
        public float GameplayTimer { get; private set; }

        public static GameSettingsController Instance;
        private void Awake()
        {
            Instance = this;
            CountDownTimer = countDownTimer;
            GameplayTimer = gameplayTimer;
        }

    }
}
