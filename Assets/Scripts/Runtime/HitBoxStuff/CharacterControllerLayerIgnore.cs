using UnityEngine;

public class CharacterControllerLayerIgnore : MonoBehaviour
{
    CharacterController _characterController;

    [SerializeField]
    [Tooltip("Layers that this CharacterController should ignore collisions with")]
    LayerMask _layersToIgnore;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        ApplyLayerIgnore();
    }

    private void ApplyLayerIgnore()
    {
        // Find all active colliders in the scene
        Collider[] allColliders = FindObjectsByType<Collider>(FindObjectsSortMode.None);

        foreach (var col in allColliders)
        {
            // Check if the collider's layer is in the ignore mask
            if (((1 << col.gameObject.layer) & _layersToIgnore) != 0)
            {
                Physics.IgnoreCollision(_characterController, col, true);
            }
        }
    }

    // Call this if you spawn new objects during gameplay
    public void RefreshIgnoredCollisions()
    {
        ApplyLayerIgnore();
    }
}