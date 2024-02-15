using System;
using System.Collections;
using Controllers;
using PlayFab;
using PlayFab.ClientModels;
using Popup;
using UnityEngine;
using Utils;
using Utils.PopupTools;

namespace Network
{
    public class PlayFabUserController : MonoBehaviour
    {
        private string _encryptedPassword;

        private MatchmakingUIController _matchmakingUIController;
        
        private bool _loginSuccessful;
        private bool _displayNameSet;
        
        private Coroutine _currentLoginCoroutine;
        private Coroutine _currentDisplayNameCoroutine;
        
        public static Action<string> ChangeDisplayNameUI;

        private void Start()
        {
            _matchmakingUIController = MatchmakingUIController.Instance;
            _matchmakingUIController.ConfirmInputBehavior(LoginType.Username, Login);
            _matchmakingUIController.GuestInputBehavior(LoginType.Guest, Login);

            StartCoroutine(InternetConnectivityChecker.CheckInternetConnectivity((isConnected) =>
            {
                if (isConnected)
                {
                    // The internet connection is available
                    bool isLoggedIn = PlayFabClientAPI.IsClientLoggedIn();

                    if (isLoggedIn)
                    {
                        _matchmakingUIController.PanelActivation(Panel.LoadingData);
                        RefreshDisplayNameAndData(false);
                    }
                    else
                    {
                        _matchmakingUIController.PanelActivation(Panel.Start);
                    }
                }
                else
                {
                    PopupManager.ShowPopup(new InformationDefinition("Internet","Please check your internet and restart the game"));
                    
                }
            }));
        }

        
        #region Login,Register,Encrypiton,Logout
        
        private void Login(LoginType type)
        {
            string username = null;
            string password = null;

            switch (type)
            {
                case LoginType.Guest:
                    username= SystemInfo.deviceName.ToLower();
                    password = username;
                    break;
                
                case LoginType.Username:
                    username = _matchmakingUIController.Username;
                    password = _matchmakingUIController.Password;
                    break;
                
                default:
                    Debug.LogWarning("Login type is different then Guest and Username");
                    return;
            }

            #region Lenght_Issues

            if (username == null)
            {
                PopupManager.ShowPopup(new InformationDefinition("Lenght",$"Please enter an Username"));
                return;
            }
            
            if (username.Length <= 3)
            {
                PopupManager.ShowPopup(new InformationDefinition("Lenght",$"The username '{username}' is too short. Usernames must be at least 3 characters long"));
                return;
            }
            
            if (password == null)
            {
                PopupManager.ShowPopup(new InformationDefinition("Lenght",$"Please enter an Username"));
                return;
            }
            
            if (password.Length <= 3)
            {
                PopupManager.ShowPopup(new InformationDefinition("Lenght",$"The password is too short. Password must be at least 3 characters long"));
                return;
            }

            #endregion

            #region StopsLooping

            if (_currentLoginCoroutine != null)
            {
                StopCoroutine(_currentLoginCoroutine);
            }

            #endregion
            
            
            
            _currentLoginCoroutine= StartCoroutine(LoginAndSetDisplayName(username, password));

        }

        private IEnumerator LoginAndSetDisplayName(string username,string password)
        {
            #region Log_In
            
            var request = new LoginWithPlayFabRequest
            {
                Username = username,
                Password = Encrypt(password)
            };

            _loginSuccessful = false;
            
            PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, (error) => OnLoginFail(error, username, password));

            while (!_loginSuccessful)
            {
                yield return null;
            }

            #region Data-Set

            //Set UsernameData
            PlayerPrefs.SetString("USERNAME",username);
            
            //Connect To Photon
            PhotonConnector.Instance.ConnectToPhoton();

            #endregion
            
            #endregion
            
            
        }

        
        private void OnLoginSuccess(LoginResult result)
        {
            
            #region RetrievePlayer Name & Data

            RefreshDisplayNameAndData(true);

            #endregion

            _loginSuccessful = true;
        }
        private void OnLoginFail(PlayFabError error,string username,string password)
        {
            if (error.Error == PlayFabErrorCode.AccountNotFound)
            {
                RegisterUser(username, password);
            }
            else
            {
                PopupManager.ShowPopup(new InformationDefinition("Failed", "Login Failed: " + error.GenerateErrorReport()));
            }
        }

        private void RegisterUser(string username, string password)
        {
            // Generate a unique email address using a placeholder domain
            string uniqueEmail = username + "@example.com";

            var registerRequest = new RegisterPlayFabUserRequest
            {
                Username = username,
                Password = Encrypt(password),
                DisplayName = username,
                Email = uniqueEmail
            };
            
            PlayFabClientAPI.RegisterPlayFabUser(registerRequest, (error)=>OnRegisterSuccess(error,username,password), OnRegisterFail);
            
        }
        
        private void OnRegisterSuccess(RegisterPlayFabUserResult result,string username,string password)
        {
            #region QuitLoops

            if (_currentLoginCoroutine != null)
            {
                StopCoroutine(_currentLoginCoroutine);
            }

            #endregion
            
            PlayFabUserData.Instance.PlayerDisplayNameData = username;//Set Display Name as First
            
            _currentLoginCoroutine= StartCoroutine(LoginAndSetDisplayName(username, password));
        }

        private void OnRegisterFail(PlayFabError error)
        {
            PopupManager.ShowPopup(new InformationDefinition("Error",$"Registration Failed: {error.GenerateErrorReport()}!"));
        }
        
        private string Encrypt(string password)
        {
            var serviceProvider = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(password);
            bs = serviceProvider.ComputeHash(bs);
            var stringBuilder = new System.Text.StringBuilder();

            foreach (var b in bs)
            {
                stringBuilder.Append(b.ToString("x2"));
            }

            return stringBuilder.ToString();
        }
        
        public void Logout()
        {
            PlayFabClientAPI.ForgetAllCredentials();

            _loginSuccessful = false;
            _displayNameSet = false;

            #region UI

            _matchmakingUIController.PanelActivation(Panel.Start);

            #endregion
            
        }

        
        
        #endregion

        #region UserAttribute

        #region UI_Display_Change_Actions
        
        public void ChangeDisplayName()
        {
            var newDisplayName = PlayFabUserData.Instance.PlayerDisplayNameData;
            
            var request = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = newDisplayName
            };

            if (newDisplayName.Length <= 3)
            {
                PopupManager.ShowPopup(new InformationDefinition("Length",$"The username '{newDisplayName}' is too short. Usernames must be at least 3 characters long"));
                return;
            }

            PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameChanged, OnDisplayNameChangeFailed);

            ChangeDisplayNameUI(newDisplayName);
        }

        private void OnDisplayNameChanged(UpdateUserTitleDisplayNameResult result)
        {
            Debug.Log("Display name changed successfully");
            PopupManager.ShowPopup(new InformationDefinition("Success",$"Display Name updated successfully!: {result.DisplayName}!"));
        }

        private void OnDisplayNameChangeFailed(PlayFabError error)
        {
            Debug.LogWarning("Failed to change display name: " + error.GenerateErrorReport());
            PopupManager.ShowPopup(new InformationDefinition("Error",$"Failed to change display name: + {error.GenerateErrorReport()!}"));
        }
        
        #endregion

        #region RefreshDisplayName

        private void RefreshDisplayNameAndData(bool firstCome)
        {
            
            var request = new GetPlayerProfileRequest
            {
                ProfileConstraints = new PlayerProfileViewConstraints
                {
                    ShowDisplayName = true
                }
            };
                
            PlayFabClientAPI.GetPlayerProfile(request, OnGetPlayerProfileSuccess, OnGetPlayerProfileError);

            if (_currentDisplayNameCoroutine != null)
            {
                StopCoroutine(_currentDisplayNameCoroutine);
            }
            
            _matchmakingUIController.PanelActivation(Panel.LoadingData);

            _currentDisplayNameCoroutine = StartCoroutine(DisplayNameCoroutine(firstCome));
        }

        private IEnumerator DisplayNameCoroutine(bool firstCome)
        {
            while (!_displayNameSet)
            {
                yield return null;
            }
            
            #region UI
            
            _matchmakingUIController.PanelActivation(Panel.MatchMaking);
            
            if (firstCome)
            {
                PopupManager.ShowPopup(new InformationDefinition("Success",$"Login successful! Welcome, {PlayFabUserData.Instance.PlayerDisplayNameData}!"));
            }
            

            #endregion
            
        }

        private void OnGetPlayerProfileSuccess(GetPlayerProfileResult result)
        {
            #region FindDisplayName(default username)
            
            PlayFabUserData.Instance.PlayerDisplayNameData = result.PlayerProfile.DisplayName;
            ChangeDisplayNameUI(PlayFabUserData.Instance.PlayerDisplayNameData);
            
            #endregion

            #region SetPlayerData

            PlayFabUserData.Instance.GetUserData();

            #endregion

            _displayNameSet = true;

        }

        private void OnGetPlayerProfileError(PlayFabError error)
        {
            Debug.LogError("Error getting player profile: " + error.GenerateErrorReport());
        }

        #endregion
        

        #endregion
        
        
    }

    public enum LoginType
    {
        Guest,
        Username
    }

}
