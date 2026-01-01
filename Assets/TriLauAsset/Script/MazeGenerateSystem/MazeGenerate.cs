using MyRule;
using UnityEngine;

namespace MyRule
{
    public class MazeGenerate : MonoBehaviour
    {
        public ShapeSpawnerManager shapeSpawnerManager;

        private void Start()
        {
            shapeSpawnerManager = GetComponent<ShapeSpawnerManager>();
            GenerateMaze();
        }

        private async void GenerateMaze()
        {
            foreach (var data in shapeSpawnerManager.spawners)
            {
                if (data.spawner != null)
                {
                    await data.spawner.SpawnShapesAlongCurve();
                }
            }
        }
    }
}
