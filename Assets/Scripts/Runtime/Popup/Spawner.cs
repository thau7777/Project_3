
using UnityEngine;


public enum KeyType
{
    Left, Down, Up, Right
}
public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject[] arrowPrefab;
    [SerializeField] private Transform leftSpawnPoint;
    [SerializeField] private int count=0;

    private void Update()
    {
        if(count<=4)
        {
            SpawnArrow();
        }
    }
    void SpawnArrow()
    {
        int index = Random.Range(0, arrowPrefab.Length);

        Instantiate(arrowPrefab[index], leftSpawnPoint.position, Quaternion.identity);

        count++;
    }

}
