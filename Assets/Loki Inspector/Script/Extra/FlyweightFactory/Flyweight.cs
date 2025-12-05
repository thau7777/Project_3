using UnityEngine;

public abstract class Flyweight : MonoBehaviour
{
    [HideInInspector]
    public FlyweightSettings settings; // Intrinsic state
    public void FlyweightInitialize(Vector3 position, Quaternion? rotation = null)
    {
        transform.position = position;
        if(rotation.HasValue)
            transform.rotation = rotation.Value;
        else
            transform.rotation = Quaternion.identity;

    }
    public void ReturnToPool()
    {
        if(gameObject.activeSelf)
        FlyweightFactory.ReturnToPool(this);
    }
    
}
