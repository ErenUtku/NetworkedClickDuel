using System;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class MatchmakingUIController : MonoBehaviour
    {
        [Header("Panels")] 
        [SerializeField] private GameObject startPanel;
        [SerializeField] private GameObject loadingDataPanel;
        [SerializeField] private GameObject matchmakingPanel;
        
        [Header("Buttons")]
        [SerializeField] private Button userConfirmButton;
        [SerializeField] private Button guestLoginButton;

        [Header("InputFields")] 
        [SerializeField] private TMP_InputField  usernameInput;
        [SerializeField] private TMP_InputField  passwordInput;
        
        [Header("Texts")] 
        [SerializeField] private TextMeshProUGUI usernameDisplayName;
        [SerializeField] private TextMeshProUGUI userWinCount;
        [SerializeField] private TextMeshProUGUI userLoseCount;
        [SerializeField] private TextMeshProUGUI userBestCount;
        
        
        private string _username;
        private string _password;

        public string Username => _username;
        public string Password => _password;
        
        public static MatchmakingUIController Instance;
        private void Awake()
        {
            Instance = this;
            
            PlayFabUserController.ChangeDisplayNameUI += ChangeDisplayName;
        }

        private void OnDestroy()
        {
            PlayFabUserController.ChangeDisplayNameUI -= ChangeDisplayName;
        }

        #region Public-Fields

        public void PanelActivation(Panel tab)
        {
            startPanel.SetActive(false);
            loadingDataPanel.SetActive(false);
            matchmakingPanel.SetActive(false);
            usernameInput.text = "";
            passwordInput.text = "";
            
            switch (tab)
            {
                case Panel.Start:
                    startPanel.SetActive(true);
                    break;
                case Panel.LoadingData:
                    loadingDataPanel.SetActive(true);
                    break;
                case Panel.MatchMaking:
                    matchmakingPanel.SetActive(true);
                    break;
                default:
                    Debug.LogWarning($"There is no such a thing called {tab}");
                    break;
            }
        }

        public void OnInputFieldChanged(string newValue)
        {
            string inputFieldName = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;

            switch (inputFieldName)
            {
                case "UsernameInputField":
                    _username = newValue;
                    Debug.LogWarning("Username: " + _username);
                    break;
                case "PasswordInputField":
                    _password = newValue;
                    Debug.LogWarning("Password: " + _password);
                    break;
                case "DisplayNameInputField":
                    PlayFabUserData.Instance.PlayerDisplayNameData = newValue;
                    break;
            }
        }

        public void ConfirmInputBehavior(LoginType loginType, Action<LoginType> onClickAction)
        {
            userConfirmButton.onClick.RemoveAllListeners();
            userConfirmButton.onClick.AddListener(() => onClickAction(loginType));
        }

        public void GuestInputBehavior(LoginType loginType, Action<LoginType> onClickAction)
        {
            guestLoginButton.onClick.RemoveAllListeners();
            guestLoginButton.onClick.AddListener(() => onClickAction(loginType));
        }

        public void UpdatePlayerData()
        {
            userWinCount.text = $"Wins :{PlayerPrefs.GetInt("Wins")}";
            userLoseCount.text = $"Lose :{PlayerPrefs.GetInt("Losses")}";
            userBestCount.text = $"Best :{PlayerPrefs.GetInt("HighScore")}";
        }
        
        #endregion

        #region Private-Fields

        private void ChangeDisplayName(string username)
        {
            usernameDisplayName.text = $"Current Name :{username}";
        }

        #endregion

    }

    public enum Panel
    {
        Start,
        LoadingData,
        MatchMaking
    }
}