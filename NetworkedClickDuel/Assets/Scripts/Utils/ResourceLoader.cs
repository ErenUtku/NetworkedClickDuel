using UnityEngine;

namespace Utils
{
    public static class ResourceLoader
    {
        #region Popups

        /// <summary>
        /// Loads pop-up GameObject according to given prefab name.
        /// </summary>
        /// <param name="prefabName">Given GameObject prefab name</param>
        /// <returns></returns>
        public static GameObject LoadPopup(string prefabName)
        {
            var targetPopup = Resources.Load("Popups/" + prefabName) as GameObject;
            return targetPopup;
        }
        
        /// <summary>
        /// Loads Pop-up Canvas.
        /// </summary>
        /// <returns></returns>
        public static GameObject LoadPopupCanvas()
        {
            var popupCanvas = Resources.Load("PopupCanvas") as GameObject;
            return popupCanvas;
        }

        #endregion
        
    
    }
}