using UnityEngine;

namespace Turnbase
{
    public class BillboardCanvas : MonoBehaviour
    {
        [SerializeField] private string priorityCameraTag = "SkillCamera"; 

        void LateUpdate()
        {
            Camera targetCam = null;

            Camera[] allCams = Camera.allCameras;

            foreach (Camera cam in allCams)
            {
                if (cam.enabled && cam.CompareTag(priorityCameraTag))
                {
                    targetCam = cam;
                    break; 
                }
            }

            if (targetCam == null)
            {
                targetCam = Camera.main;
            }

            if (targetCam != null)
            {
                transform.rotation = targetCam.transform.rotation;
            }
        }
    }
}