using System;
using Photon.Pun;
using Photon.Realtime;
using Popup;
using UnityEngine;
using Utils;
using Utils.PopupTools;

namespace Network
{
    public class PhotonConnector : MonoBehaviourPunCallbacks
    {
        public static PhotonConnector Instance;

        public static Action PlayerFound;
    
        private void Awake()
        {
            Instance = this;
        }

        #region Public-Fields
    
        public void ConnectToPhoton()
        {
            var nickName = PlayerPrefs.GetString("USERNAME");
            PhotonNetwork.AuthValues = new AuthenticationValues(nickName);
            PhotonNetwork.GameVersion = "0.0.1";
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.NickName = nickName;

            #region Score

            ExitGames.Client.Photon.Hashtable initialProps = new ExitGames.Client.Photon.Hashtable();
            initialProps["Score"] = 0;
            PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);

            #endregion
        
            PhotonNetwork.ConnectUsingSettings();
        
            Debug.Log($"Connect to Photon as {nickName}");
        }
    
        public void StartMatchmaking()
        {
            StartCoroutine(InternetConnectivityChecker.CheckInternetConnectivity((isConnected) =>
            {
                if (isConnected)
                {
                    // The internet connection is available
                    // Check if already connected to Photon
                    if (!PhotonNetwork.IsConnected)
                    {
                        ConnectToPhoton();
                    }

                    // Check if already in a room
                    if (PhotonNetwork.InRoom)
                    {
                        Debug.LogWarning("Already in a room. Cannot start matchmaking.");
                        return;
                    }

                    JoinOrCreateRoom();
                }
                else
                {
                    PopupManager.ShowPopup(new InformationDefinition("Internet","Please check your internet and restart the game"));
                }
            }));
        }


        #endregion
    
        #region PrivateFields

        private void JoinOrCreateRoom()
        {
            if (PhotonNetwork.IsConnected)
            {
                PopupManager.ShowPopup(new MatchmakingDefinition());
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                Debug.LogWarning("Not connected to the Photon Master Server yet.");
            }
        }
        private void CreatePhotonRoom(string roomName)
        {
            RoomOptions ro = new RoomOptions();
            ro.IsOpen = true;
            ro.IsVisible = true;
            ro.MaxPlayers = 2;
            PhotonNetwork.JoinOrCreateRoom(roomName, ro, TypedLobby.Default);
        }

        #endregion
    
    
        #region Photon Callbacks
        
    
        public override void OnConnectedToMaster()
        {
            Debug.Log("You have connected to the Photon Master Server");
            Debug.Log(PhotonNetwork.LocalPlayer.NickName);
        }
    
        public override void OnJoinedLobby()
        {
            Debug.Log("You have connected to a Photon Lobby");
            CreatePhotonRoom("TestRoom");
        }

        public override void OnCreatedRoom()
        {
            Debug.Log($"You have created a Photon Room named {PhotonNetwork.CurrentRoom.Name}");
        }

        public override void OnJoinedRoom()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                PlayerFound?.Invoke();
            }
        }

        public override void OnLeftRoom()
        {
            Debug.Log($"You have left a Photon Room named");
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log($"You have failed to join a Photon Room {message}, but will create a room for you");
            CreatePhotonRoom("TestRoom");
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                // Start the game
                Debug.LogWarning("All players in the room:");
    
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    Debug.LogWarning($"Player ID: {player.UserId}, Nickname: {player.NickName}");
                }

                PlayerFound?.Invoke();
            
                PhotonNetwork.LoadLevel("Game");
            
            
            }
            else
            {
                // Inform the player that they are waiting for another player
                Debug.Log("Waiting for another player to join...");
            }
        }
    

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log($"Another player has left the room {otherPlayer.UserId}");
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            Debug.Log($"Master player has left the room, new master player is {newMasterClient.UserId}");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Disconnected from server for reason " + cause.ToString());
        }

        #endregion
    }
}
