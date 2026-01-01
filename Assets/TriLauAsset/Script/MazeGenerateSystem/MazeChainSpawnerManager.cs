using UnityEngine;

namespace MyRule
{
    public class MazeChainSpawnerManager : MonoBehaviour
    {
        public GameObject chainPrefab;

        public static MazeChainSpawnerManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        public void SpawnChain(Transform parent, Vector3 start, Vector3 end)
        {
            MazeChain chain = Instantiate(chainPrefab, parent).GetComponent<MazeChain>();

            chain.Initialize(start, end);
        }
    }
}
