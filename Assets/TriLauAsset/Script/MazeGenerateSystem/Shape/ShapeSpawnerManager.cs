using UnityEngine;
using System.Collections.Generic;

namespace MyRule
{
    [System.Serializable]
    public class SpawnerData
    {
        public ShapeSpawner spawner;
        public GameObject spawnerPrefab;
        public MazePoint pointA;
        public MazePoint pointB;
        public float sideLength = 1f;
        [Range(-1f, 1f)] public float curveAmount = 0f;
        public int segmentCount = 3;
    }

    [ExecuteInEditMode]
    public class ShapeSpawnerManager : MonoBehaviour
    {
        public GameObject spawnerPrefab;
        public List<SpawnerData> spawners = new List<SpawnerData>();

        private void Update()
        {
#if UNITY_EDITOR
            foreach (var data in spawners)
            {
                if (data.spawner != null)
                {
                    if (data.spawnerPrefab != null) data.spawner.shapePrefab = data.spawnerPrefab;
                    else data.spawner.shapePrefab = spawnerPrefab;
                    data.spawner.pointA = data.pointA;
                    data.spawner.pointB = data.pointB;
                    data.spawner.sideLength = data.sideLength;
                    data.spawner.curveAmount = data.curveAmount;
                    data.spawner.segmentCount = data.segmentCount;
                }
            }
#endif
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;

            foreach (var data in spawners)
            {
                if (!data.pointA || !data.pointB) return;

                Vector3 start = data.pointA.point.position;
                Vector3 end = data.pointB.point.position;
                float distance = Vector3.Distance(start, end);
                Vector3 mid = (start + end) / 2f;
                Vector3 perpendicular = Vector3.Cross((end - start).normalized, Vector3.up);
                mid += perpendicular * distance * data.curveAmount;

                Vector3 prev = start;
                for (float t = 0; t <= 1f; t += 0.05f)
                {
                    Vector3 pos = Bezier.GetQuadraticBezierPoint(t, start, mid, end);
                    Gizmos.DrawLine(prev, pos);
                    prev = pos;
                }
            }
        }
    }
}
