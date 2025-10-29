using UnityEngine;


namespace Turnbase
{
    public class BillboardCanvas : MonoBehaviour
    {
        private Transform cam;

        void Start()
        {
            GameObject camObj = GameObject.FindGameObjectWithTag("MainCamera");
            if (camObj != null)
            {
                cam = camObj.transform;
            }
        }

        void LateUpdate()
        {
            if (cam != null)
            {
                transform.rotation = cam.rotation;
            }
        }
    }

}
