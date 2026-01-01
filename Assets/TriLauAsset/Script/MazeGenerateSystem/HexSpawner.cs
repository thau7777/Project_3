using UnityEngine;

public class HexSpawner : MonoBehaviour
{
    public GameObject hexPrefab;
    public Transform pointA;
    public Transform pointB;
    public float hexRadius = 1f;
    public bool zigzag = true;

    float HexWidth => hexRadius * Mathf.Sqrt(3);
    float HexHeight => hexRadius * 1.5f;

    readonly Vector3 Dir_Original = Vector3.right;

    void Start()
    {
        SpawnHexLine(pointA.position, pointB.position);
    }

    void SpawnHexLine(Vector3 start, Vector3 end)
    {
        Vector3 currentPos = start;
        int i = 0;

        while (Vector3.Distance(currentPos, end) > hexRadius)
        {
            Instantiate(hexPrefab, currentPos, Quaternion.identity);

            Vector3 direction = (end - currentPos).normalized;
            float angle = Vector3.SignedAngle(Dir_Original, direction, Vector3.up) * -1;
            Debug.Log("Angle: " + angle);

            Vector3 hexOffset;
            if (angle > -30 && angle <= 30) // phải
                hexOffset = new Vector3(HexWidth, 0, 0);
            else if (angle > 30 && angle <= 90) // chéo phải-trước
                hexOffset = new Vector3(HexWidth / 2, 0, HexHeight);
            else if (angle > 90 && angle <= 150) // chéo trái-trước
                hexOffset = new Vector3(HexWidth / 2, 0, HexHeight);
            else if (angle > 150 || angle <= -150) // trái
                hexOffset = new Vector3(-HexWidth, 0, 0);
            else if (angle > -150 && angle <= -90) // chéo trái-sau
                hexOffset = new Vector3(-HexWidth / 2, 0, -HexHeight);
            else // chéo phải-sau
                hexOffset = new Vector3(HexWidth / 2, 0, -HexHeight);

            if (zigzag && i % 2 == 1)
                hexOffset.z *= -1;

            currentPos += hexOffset;
            i++;

            if (i > 2000)
            {
                Debug.LogWarning("HexSpawner stopped to prevent infinite loop!");
                break;
            }
        }
    }
}
