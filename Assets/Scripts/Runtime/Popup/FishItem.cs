using UnityEngine;

public class FishItem : MonoBehaviour
{
    public FishingItemData data;

    public void AttachToHook(Transform hook)
    {
        transform.SetParent(hook.transform);
        transform.localPosition = Vector3.zero;
        //transform.localRotation = Quaternion.identity;
    }
}
