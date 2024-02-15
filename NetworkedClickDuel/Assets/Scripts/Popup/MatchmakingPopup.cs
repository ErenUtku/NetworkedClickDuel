using System;
using Network;
using Photon.Pun;
using Utils.PopupTools;

namespace Popup
{
    [Serializable]
    public class MatchmakingDefinition : PopupDefinition
    {
        public MatchmakingDefinition() : base("MatchmakingPopup")
        {
            
        }
    }

    public class MatchmakingPopup : BasicPopup
    {
        
        private InformationDefinition _definition;
       
        
        public override void InitFromDefinition(PopupDefinition givenDefinition)
        {
            base.InitFromDefinition(givenDefinition);

            _definition = givenDefinition as InformationDefinition;

            if (_definition == null)
                return;

            _definition.SetCallbacks(null, Hide);
            
        }

        private void Awake()
        {
            PhotonConnector.PlayerFound += MatchFound;
        }

        private void OnDestroy()
        {
            PhotonConnector.PlayerFound -= MatchFound;
        }


        public void OnCloseButtonClick()
        {
            PhotonNetwork.LeaveLobby();
        }

        private void MatchFound()
        {
            Destroy(this.gameObject);
        }
        
        
    }
}