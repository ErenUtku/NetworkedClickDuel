using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Utils.PopupTools;

namespace Popup
{
    [Serializable]
    public class ResultDefinition : PopupDefinition
    {
        public string playerDisplayName;
        public bool isWin;
        
        public int winCount;
        public int loseCount;
        public int highScore;
        
        public ResultDefinition(string playerDisplayName,bool isWin,int winCount,int loseCount,int highScore) : base("ResultPopup")
        {
            this.playerDisplayName = playerDisplayName;
            this.isWin = isWin;
            this.winCount = winCount;
            this.loseCount = loseCount;
            this.highScore = highScore;
        }
    }

    public class ResultPopup : BasicPopup
    {
        [Header("Text Values")]
        [SerializeField] private TextMeshProUGUI resultConditionText;
        [SerializeField] private TextMeshProUGUI playerDisplayNameText;
        [SerializeField] private TextMeshProUGUI winValueText;
        [SerializeField] private TextMeshProUGUI loseValueText;
        [SerializeField] private TextMeshProUGUI highScoreValueText;
        
        [Space]
        [Header("Buttons")] 
        [SerializeField] private Button[] closeButtons;
            
        private ResultDefinition _definition;
        
        private string _playerDisplayName;
        private bool _isWin;
        
        private int _winCount;
        private int _loseCount;
        private int _highScore;
       
        
        public override void InitFromDefinition(PopupDefinition givenDefinition)
        {
            base.InitFromDefinition(givenDefinition);

            _definition = givenDefinition as ResultDefinition;

            if (_definition == null)
                return;

            _definition.SetCallbacks(null, Hide);
            
            this._playerDisplayName = _definition.playerDisplayName;
            this._isWin = _definition.isWin;
            this._winCount = _definition.winCount;
            this._loseCount = _definition.loseCount;
            this._highScore = _definition.highScore;
            
            
            PhotonNetwork.LeaveRoom(); //Leave
            
        }

        private void Start()
        {
            #region Button Invoke Calls

            foreach (var button in closeButtons)
            {
                button.onClick?.AddListener(OnCloseButtonClick);
            }

            #endregion

            SetResultPanelUI();
        }

        private void SetResultPanelUI()
        {
            #region WinCondition

            string result;
            if (_isWin)
            {
                result = "Won";
                resultConditionText.color=Color.green;
            }
            else
            {
                result = "Lost";
                resultConditionText.color=Color.red;
            }
            
            resultConditionText.text = $"You have {result} the match!";

            #endregion

            #region PlayerData

            playerDisplayNameText.text = _playerDisplayName;
            
            winValueText.text = $"Wins : {_winCount}";
            loseValueText.text = $"Losses : {_loseCount}";
            highScoreValueText.text = $"High Score : {_highScore}";
            
            #endregion


        }


        public void OnCloseButtonClick()
        {
            SceneController.Instance.SwitchSceneIndex(0);
            Destroy(this.gameObject);
        }
        
        
    }
}