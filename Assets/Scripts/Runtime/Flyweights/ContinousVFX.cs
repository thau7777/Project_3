using UnityEngine;

public class ContinousVFX : Flyweight
{
    new ContinousVFXSettings settings => (ContinousVFXSettings) base.settings;

    public void InitializeVFX(float size)
    {
        transform.localScale = new Vector3(size, size, size);
    }


}
