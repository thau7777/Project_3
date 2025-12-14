using UnityEngine;

public class CharacterControllerLayerIgnoreController : MonoBehaviour
{
    CharacterController _characterController;

    [Tooltip("Layers that this CharacterController should ignore collisions with")]
    [SerializeField]
    private LayerMask _layersToIgnore;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        ResetLayerIgnore();
    }

    public void ApplyLayerIgnore(LayerMask layers)
    {
        _characterController.excludeLayers = layers;
    }
    public void IgnoreAllExceptGround()
    {
        // Get all layers (0-31)
        LayerMask allLayers = ~0; // All bits set to 1

        // Remove the Ground layer from the exclude mask
        int groundLayer = LayerMask.NameToLayer("Ground");
        LayerMask excludeAllButGround = allLayers & ~(1 << groundLayer);

        ApplyLayerIgnore(excludeAllButGround);
    }
    public void ResetLayerIgnore()
    {
        ApplyLayerIgnore(_layersToIgnore);
    }
}