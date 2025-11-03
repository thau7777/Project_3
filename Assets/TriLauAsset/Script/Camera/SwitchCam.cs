using UnityEngine;
using MyRule.UI;

namespace MyRule.Camera
{
    public class SwitchCam : MonoBehaviour
    {
        public GameObject cam1;
        public GameObject cam2;
        public GameObject canvas;

        private EventBinding<SwitchPanelEvent> swithPanelEventBinding;

        private void OnEnable()
        {
            swithPanelEventBinding = new EventBinding<SwitchPanelEvent>(SwithCamera);
            EventBus<SwitchPanelEvent>.Register(swithPanelEventBinding);
        }

        private void OnDisable()
        {
            EventBus<SwitchPanelEvent>.Deregister(swithPanelEventBinding);
        }

        private void Start()
        {
            cam1.SetActive(true);
            cam2.SetActive(false);
        }

        private void SwithCamera()
        {
            cam1.SetActive(!cam1.activeSelf);
            cam2.SetActive(!cam2.activeSelf);

            if (cam2.activeSelf)
            {
                canvas.SetActive(false);
            }
            else
            {
                canvas.SetActive(true);
            }
        }
    }
}