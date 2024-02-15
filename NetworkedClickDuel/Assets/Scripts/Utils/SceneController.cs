using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    public class SceneController : MonoBehaviour
    {
        public static SceneController Instance;
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Invoke(nameof(ChangeScene),4);
        }

        private void ChangeScene()
        {
            SwitchSceneIndex(1);
        }

        public void SwitchSceneIndex(int value)
        {
            SceneManager.LoadScene(value);
        }
    }
}
