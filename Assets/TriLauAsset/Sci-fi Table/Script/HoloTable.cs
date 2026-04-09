using MyRule.Audio;
using MyRule.Event;
using Unity.Cinemachine;
using UnityEngine;


namespace MyRule
{
    public class HoloTable : Singleton<HoloTable>
    {
        [SerializeField] private GameObject holoTableCam;
        [SerializeField] private ScifiMouseController scifiMouse;
        [SerializeField] private GameObject suggestUI;

        private bool playerInRange = false;

        private bool hasActive = false;

        public bool HasActive => hasActive;

        private void Start()
        {
            scifiMouse.LockMouse();
        }

        public void Interact()
        {
            if (!playerInRange) return;

            holoTableCam.SetActive(true);
            suggestUI.SetActive(true);
            scifiMouse.UnlockMouse();
            hasActive = true;

            AudioManager.Instance.PlaySound("HoloTableInteract");

            EventBus<ScifitableInteractEvent>.Raise(new ScifitableInteractEvent());
            EventBus<ShowLobbyEvent>.Raise(new ShowLobbyEvent(false));
        }

        public void Exit()
        {
            if (!playerInRange) return;
            if (!hasActive) return;

            holoTableCam.SetActive(false);
            suggestUI?.SetActive(false);
            scifiMouse.LockMouse();
            hasActive = false;

            Cursor.lockState = CursorLockMode.Locked;

            EventBus<ScifitableExitEvent>.Raise(new ScifitableExitEvent());
            EventBus<ShowLobbyEvent>.Raise(new ShowLobbyEvent(true));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (playerInRange) return;

            playerInRange = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!playerInRange) return;

            playerInRange = false;
        }
    }
}