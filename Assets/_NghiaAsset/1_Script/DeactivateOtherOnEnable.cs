using UnityEngine;

public class DeactivateOtherOnEnable : MonoBehaviour
{
    public GameObject otherObject; 

    private void OnEnable()
    {
        if (otherObject != null)
        {
            otherObject.SetActive(false);
        }
    }
}