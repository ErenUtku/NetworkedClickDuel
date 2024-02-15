using System;
using Controllers;
using Controllers.State;
using Managers;
using Network;
using Popup;
using UnityEngine;
using Utils;
using Utils.PopupTools;

namespace GameState
{
    public class LoadingState : BaseState
    {
        private float _countdownValue;
        public override void EnterState()
        {
            Debug.Log($"I am in Enter State of {this}");
            GameUIController.Instance.ChangePanelActivity(GameStates.Loading);
            _countdownValue = GameSettingsController.Instance.CountDownTimer;
        }

        public override void UpdateState()
        {
            if (_countdownValue > 0)
            {
                _countdownValue -= Time.deltaTime;
                
                int countdownInt = Mathf.FloorToInt(_countdownValue);
                GameUIController.Instance.ChangeGameTextUI(GameTextType.Countdown, countdownInt.ToString());
        
            }

            if (_countdownValue <= 1)
            {
                GameUIController.Instance.ChangeGameTextUI(GameTextType.Countdown, "Tap!!");

                if (_countdownValue < 0)
                {
                    GameStateManager.Instance.StateMachine.ChangeState(new GameplayState());
                }
            }
        }

        public override void ExitState()
        {
            Debug.Log($"I am in Exit State of {this}");
        }
        
    }
    
    public class GameplayState : BaseState
    {
        private float _gameplayTimer;
        private float _stateDelayTimer = 2f;
        private bool _gameOver;

        public static Action GameIsDone;
        public override void EnterState()
        {
            _gameplayTimer = GameSettingsController.Instance.GameplayTimer;
            GameUIController.Instance.ChangePanelActivity(GameStates.Gameplay);
            GameUIController.Instance.ChangeGameTextUI(GameTextType.Score,"0");
        }

        public override void UpdateState()
        {
            #region GameContinue

            if (_gameplayTimer > 0)
            {
                _gameplayTimer -= Time.deltaTime;
                
                int gameplayTimerInt = Mathf.FloorToInt(_gameplayTimer);
                GameUIController.Instance.ChangeGameTextUI(GameTextType.Timer, gameplayTimerInt.ToString());
        
            }

            if (_gameplayTimer <= 0)
            {
                GameUIController.Instance.ChangeGameTextUI(GameTextType.Timer, "Times up!");
                
                if (!_gameOver)
                {
                    _gameOver = true;
                    GameIsDone?.Invoke();
                    GameUIController.Instance.RedButtonChangeBehavior();
                    
                }
            }

            #endregion
            
            #region GameOver
            
            if (_gameOver)
            {
                if (_stateDelayTimer > 0)
                {
                    _stateDelayTimer -= Time.deltaTime;
                }
                else
                {
                    GameStateManager.Instance.StateMachine.ChangeState(new ResultState());
                }
            }
            
            #endregion
        }

        public override void ExitState()
        {
            Debug.Log($"Exit State of {this}");
        }
        
    }
    
    public class ResultState : BaseState
    {
        public override void EnterState()
        {
            GameUIController.Instance.ChangePanelActivity(GameStates.Result);

            bool result;
            var player1Score = ScoreComparison.Instance.Player1Score;
            var player2Score = ScoreComparison.Instance.Player2Score;

            var winCount =  PlayerPrefs.GetInt("Wins");
            var loseCount = PlayerPrefs.GetInt("Losses");
            var highScore = PlayerPrefs.GetInt("HighScore");
            
            if (player1Score > player2Score)
            {
                winCount++;
                result = true;
            }
            else
            {
                loseCount++;
                result = false;
            }

            if (player1Score > highScore)
            {
                highScore = player1Score;
            }
            
            PlayerPrefs.SetInt("Wins",winCount);
            PlayerPrefs.SetInt("Losses",loseCount);
            PlayerPrefs.SetInt("HighScore",highScore);
            PlayerPrefs.Save();
            
            PlayFabUserData.Instance.UpdatePlayerData(new PlayerData
            {
                HighScore = PlayerPrefs.GetInt("HighScore"),
                Wins = PlayerPrefs.GetInt("Wins"),
                Losses = PlayerPrefs.GetInt("Losses")
            });
            
            PopupManager.ShowPopup(new ResultDefinition(PlayFabUserData.Instance.PlayerDisplayNameData,result,winCount,loseCount,highScore));
            
        }

        public override void UpdateState()
        {
            //Debug.Log($"Update State of {this}");
        }

        public override void ExitState()
        {
            Debug.Log($"Exit State of {this}");
        }
    }
}