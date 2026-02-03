using UnityEngine;

public class BillBoardUI : MonoBehaviour
{
    [FoldoutGroup("Camera Settings")]
    [SerializeField] private bool alwaysFaceCamera = true;
    [FoldoutGroup("Camera Settings")]
    [SerializeField] private bool smoothRotation = true;
    [FoldoutGroup("Camera Settings")]
    [SerializeField] private float rotationSpeed = 10f;
    private void LateUpdate()
    {
        if (alwaysFaceCamera && Camera.main != null)
        {
            FaceCamera();
        }
    }

    /// <summary>
    /// Makes the UI always face the camera plane
    /// </summary>
    public void FaceCamera()
    {
        if (smoothRotation)
        {
            // Smooth rotation to match camera's forward direction
            Quaternion targetRotation = Quaternion.LookRotation(Camera.main.transform.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            // Instant rotation to match camera's forward direction
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }
    }
}
