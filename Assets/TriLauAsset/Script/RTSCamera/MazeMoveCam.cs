using UnityEngine;

namespace MyRule
{
    public class MazeMoveCam : MonoBehaviour
    {
        [SerializeField] private DataSO dataSO;
        [SerializeField] private Transform target;
        [SerializeField] private Transform camtarget;


        private EventBinding<CamTargetEvent> camTargetEventbinding;

        private void OnEnable()
        {
            camTargetEventbinding = new EventBinding<CamTargetEvent>(OnCamTarget);
            EventBus<CamTargetEvent>.Register(camTargetEventbinding);
        }

        private void OnDisable()
        {
            EventBus<CamTargetEvent>.Deregister(camTargetEventbinding);
        }

        private void OnCamTarget(CamTargetEvent evt)
        {
            camtarget = evt.target;
        }

        private void Update()
        {
            target.position = camtarget.position;
        }
    }
}