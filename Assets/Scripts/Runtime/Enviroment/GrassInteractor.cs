using UnityEngine;

public class GrassInteractor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool useCustomRadius = false;
    [SerializeField] private float customRadius = 2f;
    [SerializeField] private float radiusMultiplier = 6f;
    [Tooltip("Use XZ bounds size instead of trying to detect collider type")]
    [SerializeField] private bool alwaysUseBounds = false;

    private Collider colliderComponent;
    private CharacterController characterController;

    public bool IsActive { get; set; } = true;
    public float InteractionRadius { get; private set; }

    void Awake()
    {
        // Try to get regular Collider first
        colliderComponent = GetComponent<Collider>();

        // If no regular collider, try CharacterController
        if (colliderComponent == null)
        {
            characterController = GetComponent<CharacterController>();
        }

        if (colliderComponent == null && characterController == null)
        {
            Debug.LogWarning($"GrassInteractor on {gameObject.name} has no Collider or CharacterController component!", this);
        }

        CalculateInteractionRadius();
    }

    void OnEnable()
    {
        if (GrassInteractiveManager.Instance != null)
        {
            GrassInteractiveManager.Instance.RegisterInteractor(this);
        }
    }

    void OnDisable()
    {
        if (GrassInteractiveManager.Instance != null)
        {
            GrassInteractiveManager.Instance.UnregisterInteractor(this);
        }
    }

    void CalculateInteractionRadius()
    {
        if (useCustomRadius)
        {
            InteractionRadius = customRadius;
            return;
        }

        if (colliderComponent == null && characterController == null)
        {
            InteractionRadius = 0.5f;
            return;
        }

        if (alwaysUseBounds)
        {
            // Simple approach: use bounds
            InteractionRadius = GetBoundsRadius() * radiusMultiplier;
        }
        else
        {
            // Try to get specific collider radius, fallback to bounds
            InteractionRadius = GetColliderRadius() * radiusMultiplier;
        }
    }

    float GetBoundsRadius()
    {
        Bounds bounds;

        if (characterController != null)
        {
            // CharacterController doesn't have a bounds property, so calculate it
            Vector3 center = transform.position + characterController.center;
            float height = characterController.height;
            float radius = characterController.radius;

            bounds = new Bounds(center, new Vector3(radius * 2, height, radius * 2));
        }
        else if (colliderComponent != null)
        {
            bounds = colliderComponent.bounds;
        }
        else
        {
            return 0.5f;
        }

        return Mathf.Max(bounds.extents.x, bounds.extents.z);
    }

    float GetColliderRadius()
    {
        // Handle CharacterController separately
        if (characterController != null)
        {
            float maxScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.z);
            return characterController.radius * maxScale;
        }

        // Handle regular Colliders
        if (colliderComponent == null)
        {
            return 0.5f;
        }

        // Try specific collider types first for better accuracy
        switch (colliderComponent)
        {
            case CapsuleCollider capsule:
                return capsule.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.z);

            case SphereCollider sphere:
                return sphere.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);

            case BoxCollider box:
                float sizeX = box.size.x * transform.lossyScale.x;
                float sizeZ = box.size.z * transform.lossyScale.z;
                return Mathf.Max(sizeX, sizeZ) * 0.5f;

            default:
                // Fallback to bounds for mesh colliders, terrain, etc.
                Bounds bounds = colliderComponent.bounds;
                return Mathf.Max(bounds.extents.x, bounds.extents.z);
        }
    }

    void OnValidate()
    {
        if (colliderComponent == null)
        {
            colliderComponent = GetComponent<Collider>();
        }

        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }

        CalculateInteractionRadius();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 pos = transform.position;
        //pos.y += 1f;
        Gizmos.DrawWireSphere(pos, InteractionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, pos);

        // Draw the actual collider/character controller outline
        DrawColliderOutline();
    }

    void DrawColliderOutline()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);

        if (characterController != null)
        {
            // Draw CharacterController capsule
            Vector3 center = transform.position + characterController.center;
            float radius = characterController.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.z);
            float height = characterController.height * transform.lossyScale.y;

            // Draw top and bottom spheres
            Gizmos.DrawWireSphere(center + Vector3.up * (height * 0.5f - radius), radius);
            Gizmos.DrawWireSphere(center + Vector3.down * (height * 0.5f - radius), radius);

            // Draw cylinder lines
            Vector3 topFront = center + Vector3.up * (height * 0.5f - radius) + Vector3.forward * radius;
            Vector3 bottomFront = center + Vector3.down * (height * 0.5f - radius) + Vector3.forward * radius;
            Gizmos.DrawLine(topFront, bottomFront);

            Vector3 topBack = center + Vector3.up * (height * 0.5f - radius) + Vector3.back * radius;
            Vector3 bottomBack = center + Vector3.down * (height * 0.5f - radius) + Vector3.back * radius;
            Gizmos.DrawLine(topBack, bottomBack);

            Vector3 topRight = center + Vector3.up * (height * 0.5f - radius) + Vector3.right * radius;
            Vector3 bottomRight = center + Vector3.down * (height * 0.5f - radius) + Vector3.right * radius;
            Gizmos.DrawLine(topRight, bottomRight);

            Vector3 topLeft = center + Vector3.up * (height * 0.5f - radius) + Vector3.left * radius;
            Vector3 bottomLeft = center + Vector3.down * (height * 0.5f - radius) + Vector3.left * radius;
            Gizmos.DrawLine(topLeft, bottomLeft);
        }
        else if (colliderComponent is BoxCollider box)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(box.center, box.size);
            Gizmos.matrix = Matrix4x4.identity;
        }
        else if (colliderComponent is SphereCollider sphere)
        {
            Gizmos.DrawWireSphere(transform.position + sphere.center, sphere.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z));
        }
        else if (colliderComponent is CapsuleCollider capsule)
        {
            Vector3 center = transform.position + capsule.center;
            float radius = capsule.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.z);
            Gizmos.DrawWireSphere(center, radius);
        }
    }

    // Call this if you need to update radius at runtime
    public void UpdateRadius()
    {
        CalculateInteractionRadius();
    }
}