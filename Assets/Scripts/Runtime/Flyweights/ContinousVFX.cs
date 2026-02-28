using UnityEngine;

public class ContinousVFX : Flyweight
{
    new ContinousVFXSettings settings => (ContinousVFXSettings) base.settings;

    public void InitializeVFX(float size)
    {
        gameObject.SetActive(true);
        transform.localScale = new Vector3(size, size, size);
    }


}
