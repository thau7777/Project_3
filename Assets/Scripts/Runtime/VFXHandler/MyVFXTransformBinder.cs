using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// Custom VFX Transform Binder that can be set at runtime
/// Binds a Transform to VFX Graph Transform properties (position, angles, scale)
/// </summary>
[RequireComponent(typeof(VisualEffect))]
public class MyVFXTransformBinder : MonoBehaviour
{
    [Header("Binding Settings")]
    [Tooltip("The transform to bind to the VFX Graph")]
    public Transform Target;

    [Tooltip("The property name in the VFX Graph (without _position/_angles/_scale suffix)")]
    public string PropertyName = "Transform";

    [Header("Update Settings")]
    [Tooltip("Update the binding every frame")]
    public bool ContinuousUpdate = true;

    [Tooltip("Only update position (ignore rotation and scale)")]
    public bool PositionOnly = false;

    private VisualEffect _vfx;
    private int _positionID;
    private int _anglesID;
    private int _scaleID;

    private void Awake()
    {
        _vfx = GetComponent<VisualEffect>();
        CachePropertyIDs();
    }

    private void OnEnable()
    {
        // Update immediately when enabled
        if (Target != null)
        {
            UpdateBinding();
        }
    }

    private void LateUpdate()
    {
        if (ContinuousUpdate && Target != null)
        {
            UpdateBinding();
        }
    }

    /// <summary>
    /// Cache property IDs for better performance
    /// </summary>
    private void CachePropertyIDs()
    {
        _positionID = Shader.PropertyToID(PropertyName + "_position");
        _anglesID = Shader.PropertyToID(PropertyName + "_angles");
        _scaleID = Shader.PropertyToID(PropertyName + "_scale");
    }

    /// <summary>
    /// Update the VFX Graph with the current target transform values
    /// </summary>
    public void UpdateBinding()
    {
        if (_vfx == null || Target == null) return;

        // Always update position
        _vfx.SetVector3(_positionID, Target.position);

        if (!PositionOnly)
        {
            // Update rotation and scale
            _vfx.SetVector3(_anglesID, Target.localEulerAngles);
            _vfx.SetVector3(_scaleID, Target.localScale);
        }
    }

    /// <summary>
    /// Set the target transform at runtime
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        Target = newTarget;
        if (newTarget != null)
        {
            UpdateBinding();
        }
    }

    /// <summary>
    /// Change the property name and recache IDs
    /// </summary>
    public void SetPropertyName(string newPropertyName)
    {
        PropertyName = newPropertyName;
        CachePropertyIDs();
        if (Target != null)
        {
            UpdateBinding();
        }
    }

    /// <summary>
    /// Check if the binding is valid
    /// </summary>
    public bool IsValid()
    {
        return _vfx != null && Target != null && !string.IsNullOrEmpty(PropertyName);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Update property IDs when changed in inspector
        if (Application.isPlaying && _vfx != null)
        {
            CachePropertyIDs();
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a line from VFX to target in editor
        if (Target != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, Target.position);
            Gizmos.DrawWireSphere(Target.position, 0.1f);
        }
    }
#endif
}