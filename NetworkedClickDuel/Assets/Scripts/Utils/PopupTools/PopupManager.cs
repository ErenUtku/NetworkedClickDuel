using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utils.PopupTools
{
    public class PopupManager : MonoBehaviour
    {
        private static readonly List<GameObject> PopupList = new List<GameObject>();
        private static Transform _popupTargetCanvas;
        private static Image _popupCanvasBackground;
        
        private void Awake()
        {
            var popupCanvasObject = ResourceLoader.LoadPopupCanvas();
            _popupTargetCanvas = Instantiate(popupCanvasObject).transform;
            _popupCanvasBackground = _popupTargetCanvas.GetComponent<Image>();
            DontDestroyOnLoad(_popupTargetCanvas);
            DontDestroyOnLoad(gameObject);
        }
    
        private static void CloseAll()
        {
            for (var i = 0; i < PopupList.Count; i++)
            {
                var popup = PopupList[i];
                PopupList.RemoveAt(i);
                Destroy(popup);
            }
        }

        public static void ShowPopup(PopupDefinition popupDefinition)
        {
            CloseAll();
            
            _popupCanvasBackground.enabled = true;
            _popupCanvasBackground.raycastTarget = true;
        
            var loadedPopup = ResourceLoader.LoadPopup(popupDefinition.PrefabName);
            var popupGameObject = Instantiate(loadedPopup, _popupTargetCanvas);
            
            var iPopup = popupGameObject.GetComponent<IPopup>();
        
            iPopup.InitFromDefinition(popupDefinition);

            PopupList.Add(popupGameObject);
            
        }

        public static void ClosePopupBackground()
        {
            _popupCanvasBackground.raycastTarget = false;
            _popupCanvasBackground.enabled = false;
        }
    }
}