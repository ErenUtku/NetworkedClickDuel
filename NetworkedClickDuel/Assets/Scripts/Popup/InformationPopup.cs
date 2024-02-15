using System;
using TMPro;
using UnityEngine;
using Utils.PopupTools;

namespace Popup
{
    [Serializable]
    public class InformationDefinition : PopupDefinition
    {
        public string infoType;
        public string infoTextValue;
        public InformationDefinition(string infoType,string infoTextValue) : base("InformationPopup")
        {
            this.infoTextValue = infoTextValue;
            this.infoType = infoType;
        }
    }

    public class InformationPopup : BasicPopup
    {
        [SerializeField] private TextMeshProUGUI typeText;
        [SerializeField] private TextMeshProUGUI infoText;
        
        private InformationDefinition _definition;
        private string _typeTextValue;
        private string _infoTextValue;
        
        public override void InitFromDefinition(PopupDefinition givenDefinition)
        {
            base.InitFromDefinition(givenDefinition);

            _definition = givenDefinition as InformationDefinition;

            if (_definition == null)
                return;

            _definition.SetCallbacks(null, Hide);

            _typeTextValue = _definition.infoTextValue;
            _infoTextValue = _definition.infoType;
        }

        private void Start()
        {
            typeText.text = _typeTextValue;
            infoText.text = _infoTextValue;
        }

        public void OnCloseButtonClick()
        {
            _definition.CloseCallback?.Invoke();
        }
        
        
    }
}