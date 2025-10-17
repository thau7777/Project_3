using UnityEngine;

public abstract class Flyweight : MonoBehaviour
{
    public FlyweightSettings settings; // Intrinsic state
    public void Initialize(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }
}
