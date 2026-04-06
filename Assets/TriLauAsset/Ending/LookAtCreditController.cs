using UnityEngine;

namespace MyRule
{
    public class LookAtCreditController : MonoBehaviour
    {
        [SerializeField] private Transform target;

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 direction = target.position - transform.position;
            direction.y = 0;

            if (direction.sqrMagnitude < 0.001f) return;

            Quaternion rot = Quaternion.LookRotation(direction);
            transform.rotation = rot * Quaternion.Euler(0, 180f, 0);
        }
    }
}