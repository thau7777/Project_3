using MyRule.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyRule
{
    public class Planet : MonoBehaviour
    {
        public PlanetSO planetSO;
        public GameObject highlightRing;
        public float moveSpeed = 3f;

        private Vector3 baseLocalPos;
        private Vector3 baseLocalScale;

        public Vector3 detailLocalPos;
        public Vector3 detailLocalScale;

        public bool isDetailShown;

        private const float SNAP_THRESHOLD = 0.0001f;

        private bool canActive = false;
        private EventBinding<ScifitableActiveEvent> eventBinding;

        private void OnEnable()
        {
            eventBinding = new EventBinding<ScifitableActiveEvent>(ActivePlanet);
            EventBus<ScifitableActiveEvent>.Register(eventBinding);
        }

        private void OnDisable()
        {
            EventBus<ScifitableActiveEvent>.Deregister(eventBinding);
            eventBinding = null;
        }

        private void Start()
        {
            baseLocalPos = transform.localPosition;
            baseLocalScale = transform.localScale;
        }

        private void Update()
        {
            Vector3 targetPos = isDetailShown ? detailLocalPos : baseLocalPos;
            Vector3 targetScale = isDetailShown ? detailLocalScale : baseLocalScale;

            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                targetPos,
                moveSpeed * Time.deltaTime
            );

            transform.localScale = Vector3.Lerp(
                transform.localScale,
                targetScale,
                moveSpeed * Time.deltaTime
            );

            SnapIfClose(targetPos, targetScale);
        }

        private void SnapIfClose(Vector3 targetPos, Vector3 targetScale)
        {
            if (Vector3.Distance(transform.localPosition, targetPos) < SNAP_THRESHOLD)
                transform.localPosition = targetPos;

            if (Vector3.Distance(transform.localScale, targetScale) < SNAP_THRESHOLD)
                transform.localScale = targetScale;
        }

        public void ShowDetailPlanet()
        {
            highlightRing.SetActive(false);
            isDetailShown = true;
            canActive = true;

            PlanetView.Instance.Show(planetSO);
        }

        public void HideDetailPlanet()
        {
            highlightRing.SetActive(true);
            isDetailShown = false;
            canActive = false;

            PlanetView.Instance.Hide();
        }

        private void ActivePlanet(ScifitableActiveEvent obj)
        {
            if (!canActive) return;

            //MapTypeManager.Instance.SetMapType(planetSO.planetType);

            PortalManager.Instance.SetTargetScene(planetSO.scene);

            Cursor.lockState = CursorLockMode.None;
        }
    }
}
