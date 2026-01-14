using Unity.Cinemachine;
using UnityEngine;


namespace MyRule
{
    public class HoloTable : MonoBehaviour
    {
        [SerializeField] private GameObject holoTableCam;
        [SerializeField] private ScifiMouseController scifiMouse;
        [SerializeField] private GameObject keyObj;

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
            scifiMouse.UnlockMouse();
            hasActive = true;
            keyObj.SetActive(false);

            EventBus<ScifitableInteractEvent>.Raise(new ScifitableInteractEvent());
        }

        public void Exit()
        {
            if (!playerInRange) return;
            if (!hasActive) return;

            holoTableCam.SetActive(false);
            scifiMouse.LockMouse();
            hasActive = false;
            keyObj?.SetActive(true);

            EventBus<ScifitableExitEvent>.Raise(new ScifitableExitEvent());
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