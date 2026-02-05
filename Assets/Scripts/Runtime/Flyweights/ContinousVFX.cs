using UnityEngine;

public class ContinousVFX : Flyweight
{
    new ContinousVFXSettings settings => (ContinousVFXSettings) base.settings;

    public void InitializeVFX(float size, Transform parent = null)
    {
        gameObject.SetActive(true);
        transform.localScale = new Vector3(size, size, size);

        if(parent != null)
        {
            transform.SetParent(parent);
        }
    }


}
