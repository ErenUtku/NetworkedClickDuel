using Controllers.State;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class GameUIController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private GameObject gameplayPanel;
        [SerializeField] private GameObject resultPanel;
    
        [Space]
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI countdownText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI scoreText;

        [Space] 
        [Header("Button")] [SerializeField] private Button redButton;

        public static GameUIController Instance;
        private void Awake()
        {
            Instance = this;

            PlayerController.ScoreChange += ChangeGameTextUI;
        }

        public void ChangeGameTextUI(GameTextType type,string value)
        {
            TextMeshProUGUI selectedText = null;
            switch (type)
            {
                case GameTextType.Countdown:
                    selectedText = countdownText;
                    break;
                case GameTextType.Timer:
                    selectedText = timerText;
                    break;
                case GameTextType.Score:
                    selectedText = scoreText;
                    break;
                default:
                    Debug.LogWarning("No Text type found");
                    break;
            }

            if (selectedText != null)
            {
                selectedText.text = value;
            }
        }

        public void ChangePanelActivity(GameStates type)
        {
            loadingPanel.SetActive(false);
            gameplayPanel.SetActive(false);
            resultPanel.SetActive(false);
        
            switch(type)
            {
                case GameStates.Loading:
                    loadingPanel.SetActive(true);
                    break;
                case GameStates.Gameplay:
                    gameplayPanel.SetActive(true);
                    break;
                case GameStates.Result:
                    resultPanel.SetActive(true);
                    break;
                default:
                    Debug.LogWarning($"There is no such a/an {type} state");
                    break;
            }
        }

        public void RedButtonChangeBehavior()
        {
            redButton.interactable = false;
            redButton.transform.GetChild(0).gameObject.SetActive(true); //STOP TEXT
        }
    }

    public enum GameTextType
    {
        Countdown,
        Timer,
        Score
    }
}