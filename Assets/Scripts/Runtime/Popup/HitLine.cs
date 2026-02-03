using UnityEngine;

public class HitLine : MonoBehaviour
{
    public float yPos;

    void Awake()
    {
        yPos = transform.position.y;
    }
}
