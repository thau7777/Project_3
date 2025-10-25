using UnityEngine;

namespace MyRule
{
    public class MainMenuSwithCam : MonoBehaviour
    {
        public GameObject cam1;
        public GameObject cam2;
        public GameObject canvas;

        private void Start()
        {
            cam1.SetActive(true);
            cam2.SetActive(false);
        }

        public void SwithCamera()
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