using UnityEngine;

namespace MyRule
{
    public class EscManager : MonoBehaviour
    {
        public static EscManager Instance;

        public InputReader inputReader;
        public AudioSource escSound;

        private bool isSettingsOpen = false;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            inputReader.uiActions.onEscape += EscPanel;
        }

        private void OnDisable()
        {
            inputReader.uiActions.onEscape -= EscPanel;
        }

        public void SetIsSettingsOpen(bool isOpen)
        {
            isSettingsOpen = isOpen;
        }

        private void EscPanel()
        {
            if (isSettingsOpen)
            {
                UISettingsManager.Instance.HideSettingsPanel();
                MainMenuSwithCam.Instance.SwithCamera();
                escSound.Play();
            }
        }
    }
}