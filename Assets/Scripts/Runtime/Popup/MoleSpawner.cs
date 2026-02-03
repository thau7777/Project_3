using UnityEngine;

public class MoleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject molePrefab;
    [SerializeField] private Transform[] holePos;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float initialDelay = 1f;
    private void Start()
    {
        InvokeRepeating(nameof(SpawnMole), initialDelay, spawnInterval);
    }
    private void SpawnMole()
    {
        int randomIndex = Random.Range(0, holePos.Length);
        Instantiate(molePrefab, holePos[randomIndex].position, Quaternion.identity);
    }
}
