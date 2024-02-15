using System;
using System.Collections.Generic;
using Controllers;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using Utils;

namespace Network
{
    public class PlayFabUserData : MonoSingletonPersistent<PlayFabUserData>
    {
        private string _playerDisplayName;
        public string PlayerDisplayNameData
        {
            get => _playerDisplayName;
            set => _playerDisplayName = value;
        }

        #region GetData

        public void GetUserData()
        {
            PlayerPrefs.DeleteAll();
        
            var request = new GetUserDataRequest();

            PlayFabClientAPI.GetUserData(request, OnGetUserDataSuccess, OnGetUserDataFailure);
        }

        private void OnGetUserDataSuccess(GetUserDataResult result)
        {
            var playerData = new PlayerData();

            if (result.Data.TryGetValue("HighScore", out var highScoreValue))
            {
                if (int.TryParse(highScoreValue.Value, out var highScore))
                {
                    playerData.HighScore = highScore;
            
                    // Set the retrieved high score in PlayerPrefs
                    PlayerPrefs.SetInt("HighScore", highScore);
                    PlayerPrefs.Save();
                }
            }

            if (result.Data.TryGetValue("Win", out var winValue))
            {
                if (int.TryParse(winValue.Value, out var wins))
                {
                    playerData.Wins = wins;
            
                    // Set the retrieved win count in PlayerPrefs
                    PlayerPrefs.SetInt("Wins", wins);
                    PlayerPrefs.Save();
                }
            }

            if (result.Data.TryGetValue("Lose", out var loseValue))
            {
                if (int.TryParse(loseValue.Value, out var losses))
                {
                    playerData.Losses = losses;
            
                    // Set the retrieved loss count in PlayerPrefs
                    PlayerPrefs.SetInt("Losses", losses);
                    PlayerPrefs.Save();
                }
            }
        
            MatchmakingUIController.Instance.UpdatePlayerData();
        }


        private void OnGetUserDataFailure(PlayFabError error)
        {
            Debug.LogError($"Failed to get user data: {error.ErrorMessage}");
        }

        #endregion
    
        #region SetData

        public void UpdatePlayerData(PlayerData playerData)
        {
            // Create a request to update user data
            var request = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
                {
                    { "HighScore", playerData.HighScore.ToString() },
                    { "Win", playerData.Wins.ToString() },
                    { "Lose", playerData.Losses.ToString() }
                }
            };

            // Send the request to PlayFab
            PlayFabClientAPI.UpdateUserData(request, OnUpdateUserDataSuccess, OnUpdateUserDataFailure);
        }

        private void OnUpdateUserDataSuccess(UpdateUserDataResult result)
        {
            Debug.Log("Player data updated successfully!");
        }

        private void OnUpdateUserDataFailure(PlayFabError error)
        {
            Debug.LogError($"Failed to update player data: {error.ErrorMessage}");
        }

        #endregion

    
    
    }

    [Serializable]
    public class PlayerData
    {
        public int HighScore { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
    }
}