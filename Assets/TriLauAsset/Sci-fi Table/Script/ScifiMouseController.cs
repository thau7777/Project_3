using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyRule
{
    public class ScifiMouseController : MonoBehaviour
    {
        public static ScifiMouseController instance;

        public Transform ringTransform;

        public float speed = 5f;

        private Rigidbody rb;
        private Vector2 moveInput;

        public bool isOnPlanet = false;
        private Vector3 planetPos;

        private bool isMouseLocked = false;

        private Vector3 mousePos;

        private EventBinding<ScifiMouseMoveEvent> moveBinding;

        private void OnEnable()
        {
            moveBinding = new EventBinding<ScifiMouseMoveEvent>(OnMove);
            EventBus<ScifiMouseMoveEvent>.Register(moveBinding);
        }

        private void OnDisable()
        {
            EventBus<ScifiMouseMoveEvent>.Deregister(moveBinding);
            moveBinding = null;
        }

        void Awake()
        {
            instance = this;

            rb = GetComponent<Rigidbody>();
        }

        private void OnMove(ScifiMouseMoveEvent evt)
        {
            moveInput = evt.mousePosition;
        }
        
        void FixedUpdate()
        {
            if (isMouseLocked) return;

            Vector3 moveDirection = new Vector3(
                moveInput.x,
                0f,
                moveInput.y
            );

            Vector3 targetPos = rb.position + moveDirection * speed * Time.fixedDeltaTime;
            rb.MovePosition(targetPos);

            if (isOnPlanet)
            {
                ringTransform.position = Vector3.Lerp(ringTransform.position, planetPos, Time.fixedDeltaTime * 10f);
            }
            else
            {
                ringTransform.position = new Vector3(
                               targetPos.x,
                                              targetPos.y,
                                                             targetPos.z
                                                                        );
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Planet"))
            {
                isOnPlanet = true;
                planetPos = other.transform.position;
                PlanetManager.instance.planetTargetd = other.GetComponent<Planet>();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Planet"))
            {
                isOnPlanet = false;
            }
        }

        public void LockMouse()
        {
            isMouseLocked = true;
            ringTransform.gameObject.SetActive(false);
            mousePos = transform.position;
        }

        public void UnlockMouse()
        {
            isMouseLocked = false;
            ringTransform.gameObject.SetActive(true);
            transform.position = mousePos;
        }
    }
}
