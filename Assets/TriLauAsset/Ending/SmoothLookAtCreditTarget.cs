using UnityEngine;

namespace MyRule
{
    public class SmoothLookAtCredit : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float rotationSpeed = 5f;

        private void Update()
        {
            if (target == null) return;

            Vector3 direction = target.position - transform.position;

            if (direction.sqrMagnitude < 0.001f) return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        public void SetTarget(Transform target) => this.target = target;
    }
}