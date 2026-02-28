using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(MyVFXTransformBinder))]
public class CharacterEffectController : MonoBehaviour
{
    [SerializeField]
    private VisualEffect _vfxGraph;

    [SerializeField]
    private MyVFXTransformBinder _transformBinder;

    private void Awake()
    {
        _vfxGraph = GetComponent<VisualEffect>();

        // Get or add the transform binder
        _transformBinder = GetComponent<MyVFXTransformBinder>();
        if (_transformBinder == null)
        {
            _transformBinder = gameObject.AddComponent<MyVFXTransformBinder>();
            _transformBinder.PropertyName = "Transform";
            _transformBinder.ContinuousUpdate = true;
        }
    }

    public void SetupCharacterEffect(Transform parent)
    {
        if (parent == null) return;

        SkinnedMeshRenderer skinnedMeshRenderer = parent.GetComponent<SkinnedMeshRenderer>();
        _vfxGraph.SetSkinnedMeshRenderer("SkinnedMeshRenderer", skinnedMeshRenderer);

        var targetTransform = skinnedMeshRenderer.transform.Find("CharacterEffectTarget");

        if (targetTransform != null)
        {
            if(_transformBinder == null)
            {
                Debug.LogError("Transform binder not found on CharacterEffectController!");
                return;
            }
            // Set the target - the binder will handle updates automatically
            _transformBinder.SetTarget(targetTransform);
            Debug.Log("Successfully bound transform: " + targetTransform.name);
        }
        else
        {
            Debug.LogWarning("CharacterEffectTarget not found!");
        }
    }
}