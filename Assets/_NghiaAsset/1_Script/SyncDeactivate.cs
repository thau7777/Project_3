using UnityEngine;

namespace Turnbase
{
    public class SyncDeactivate : MonoBehaviour
    {
        public GameObject parentPanel;
        private bool isProcessing = false;

        private void OnDisable()
        {
            if (isProcessing) return; 

            if (parentPanel != null && parentPanel.activeSelf)
            {
                isProcessing = true;
                parentPanel.SetActive(false);
                isProcessing = false;
            }
        }
    }
}