using UnityEngine;


namespace MyRule
{
    public class LookAtCamera : MonoBehaviour
    {
        [SerializeField] private Transform obj;

        private bool targetInrange = false;

        void Update()
        {
            if (!targetInrange) return;

            obj.LookAt(transform.position + Camera.main.transform.forward);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (targetInrange) return;
            targetInrange = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!targetInrange) return;
            targetInrange = false;

            transform.rotation = Quaternion.identity;
        }
    }
}