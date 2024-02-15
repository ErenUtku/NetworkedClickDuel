using System;
using System.Collections;
using PlayFab;
using PlayFab.MultiplayerModels;
using UnityEngine;

namespace Network
{
    [Obsolete]
    public class PlayFabMatchmakingController : MonoBehaviour
    {
    
        private const string QueueName = "DefaultQueue";

        private string _ticketId;
    
        private Coroutine _pollTicketCoroutine;

        public void StartMatchmaking()
        {
            //Disable mm button, active search button
            ////TODO
        
            PlayFabMultiplayerAPI.CreateMatchmakingTicket(
                new CreateMatchmakingTicketRequest 
                {
           
                    Creator = new MatchmakingPlayer 
                    {
                        Entity = new EntityKey 
                        {
                            Id = "Id_Player",//PlayFabUserData.EntityId,
                            Type = "title_player_account",
                        }, 
                        Attributes = new MatchmakingPlayerAttributes 
                        {
                            DataObject = new { }
                        }
                    },
                
                    GiveUpAfterSeconds = 120,
                
                    QueueName = QueueName,
                
                },
                OnMatchmakingTicketCreated,
                OnMatchmakingError
            );
        }

        private void OnMatchmakingTicketCreated(CreateMatchmakingTicketResult result)
        {
            _ticketId = result.TicketId;
            _pollTicketCoroutine = StartCoroutine(PollTicket());

            //LeaveQueueButton ON
            ////TODO


        }

        private void OnMatchmakingError(PlayFabError error)
        {
            Debug.LogWarning(error.GenerateErrorReport());
        }

        private IEnumerator PollTicket()
        {
            while (true)
            {
                PlayFabMultiplayerAPI.GetMatchmakingTicket(
                    new GetMatchmakingTicketRequest
                    {
                        TicketId = _ticketId,
                        QueueName = QueueName
                    },
                    OnGetMatchmakingTicket,
                    OnMatchmakingError
                );

                yield return new WaitForSeconds(6f);

            }
        
        }

        private void OnGetMatchmakingTicket(GetMatchmakingTicketResult result)
        {
            switch (result.Status)
            {
                case "Matched":
                    StopCoroutine(_pollTicketCoroutine);
                    StartMatch(result.MatchId);
                    break;
            
                case "Canceled":
                    break;
            }
        
        }

        private void StartMatch(string matchId)
        {
            PlayFabMultiplayerAPI.GetMatch(
                new GetMatchRequest
                {
                    MatchId = matchId,
                    QueueName = QueueName
                },
                OnGetMatch,
                OnMatchmakingError
            );
        }

        private void OnGetMatch(GetMatchResult result)
        {
            var player1 = result.Members[0].Entity.Id;
            var player2 = result.Members[1].Entity.Id;
        
        
            Debug.Log($"{player1} vs {player2}");

            //SceneController.Instance.SwitchSceneGame();

        }
    
        private void OnMatchmakingError()
        {
        
        }
    }
}

