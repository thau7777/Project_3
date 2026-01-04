using UnityEngine;

namespace MyRule
{
    public class RTSCameraController : MonoBehaviour
    {
        public Transform target;
        public Transform player;
        public float moveSpeed = 5f;

        private EventBinding<CamTargetEvent> camTargetEventBinding;

        private void OnEnable()
        {
            camTargetEventBinding = new EventBinding<CamTargetEvent>(OnCamTargetEvent);
            EventBus<CamTargetEvent>.Register(camTargetEventBinding);
        }

        private void OnDisable()
        {
            EventBus<CamTargetEvent>.Deregister(camTargetEventBinding);
        }

        private void FixedUpdate()
        {
            target.position = Vector3.Lerp(target.position, player.position, moveSpeed * Time.fixedTime);
        }

        private void OnCamTargetEvent(CamTargetEvent evt)
        {
            player = evt.target;
        }
    }
}
