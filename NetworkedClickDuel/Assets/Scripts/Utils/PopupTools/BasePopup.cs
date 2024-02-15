using DG.Tweening;
using UnityEngine;

namespace Utils.PopupTools
{
    public class BasicPopup : MonoBehaviour ,IPopup
    {
        protected GameObject GameObjectRef;
        protected PopupDefinition PopupDefinition;
    
        public virtual void InitFromDefinition(PopupDefinition definition)
        {
            GameObjectRef = this.gameObject;
            PopupDefinition = definition;

            Show();
            
            transform.DOScale(Vector3.one, .25f);
        }
        public void Show()
        {
            transform.localScale = Vector3.zero;
            if(!this.gameObject.activeSelf)
                gameObject.SetActive(true);
        }
        public void Hide()
        {
            PopupManager.ClosePopupBackground();
            transform.DOScale(Vector3.zero, .25f)
                .OnComplete(() =>
                {
                    Destroy();
                });
        }

        protected virtual void OnDisable()
        {
            PopupManager.ClosePopupBackground();
            Hide();
        }

        private void Destroy()
        {
            Destroy(this.gameObject);
        }
    
    }
}