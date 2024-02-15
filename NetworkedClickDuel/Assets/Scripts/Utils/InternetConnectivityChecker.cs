using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Utils
{
    public class InternetConnectivityChecker : MonoBehaviour
    {
        public static IEnumerator CheckInternetConnectivity(System.Action<bool> onResult)
        {
            using UnityWebRequest www = UnityWebRequest.Get("http://www.google.com");
            
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                onResult(false);
            }
            else
            {
                onResult(true);
            }
        }
    }
}