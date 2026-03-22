using UnityEngine;

public class ParryEffect : MonoBehaviour
{
    public static ParryEffect Instance;

    void Awake() => Instance = this;

    public void PlayEffect(GameObject prefab, Transform targetTransform, float duration = 2f)
    {
        if (prefab == null || targetTransform == null) return;

        GameObject effect = Instantiate(prefab, targetTransform.position, targetTransform.rotation);

        effect.transform.SetParent(targetTransform);

        Destroy(effect, duration);
    }
}