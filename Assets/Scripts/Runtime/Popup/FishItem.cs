using UnityEngine;

public class FishItem : MonoBehaviour
{
    [SerializeField] public FishingItemData data;
    [SerializeField] private HookController hook;

    
    public void AttachToHook(Transform hook)
    {
        transform.SetParent(hook.transform);
        transform.localPosition = Vector3.zero;
        //transform.localRotation = Quaternion.identity;
    }
}
