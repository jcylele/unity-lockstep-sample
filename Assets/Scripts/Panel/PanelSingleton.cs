using UnityEngine;

namespace Panel
{
    public class PanelSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        protected void Awake()
        {
            Instance = this as T;
        }

        public void ShowGame(bool show)
        {
            GamePanel.Instance.gameObject.SetActive(show);
            BeginPanel.Instance.gameObject.SetActive(!show);
            ReplayPanel.Instance.gameObject.SetActive(!show);
        }
    }
}
