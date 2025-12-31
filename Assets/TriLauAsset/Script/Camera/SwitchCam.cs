using UnityEngine;
using MyRule.UI;

namespace MyRule.Camera
{
    public class SwitchCam : MonoBehaviour
    {
        public GameObject cam1;
        public GameObject cam2;
        public GameObject cam3;

        private EventBinding<SwitchCamEvent> switchCamEventBinding;

        private void OnEnable()
        {
            switchCamEventBinding = new EventBinding<SwitchCamEvent>(SwithCamera);
            EventBus<SwitchCamEvent>.Register(switchCamEventBinding);
        }

        private void OnDisable()
        {
            EventBus<SwitchCamEvent>.Deregister(switchCamEventBinding);
        }

        private void Start()
        {
            cam1.SetActive(true);
            cam2.SetActive(false);
            cam3.SetActive(false);
        }

        private void SwithCamera(SwitchCamEvent evt)
        {
            cam1.SetActive(false);
            cam2.SetActive(false);
            cam3.SetActive(false);

            if (evt.Cam == 1)
            {
                cam1.SetActive(true);
            }
            else if (evt.Cam == 2)
            {
                cam2.SetActive(true);
            }
            else if (evt.Cam == 3)
            {
                cam3.SetActive(true);
            }
        }
    }
}