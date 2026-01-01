using Cysharp.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Rendering.ProbeAdjustmentVolume;

namespace MyRule
{
    public class ShapeSpawner : MonoBehaviour
    {
        public GameObject shapePrefab;
        public MazePoint pointA;
        public MazePoint pointB;
        public float sideLength = 1f;
        [Range(-1f, 1f)] public float curveAmount = 0.3f;
        public int segmentCount = 20;

        public UniTask SpawnShapesAlongCurve()
        {
            Vector3 start = pointA.point.position;
            Vector3 end = pointB.point.position;
            float distance = Vector3.Distance(start, end);

            Vector3 mid = (start + end) / 2f;
            Vector3 perpendicular = Vector3.Cross((end - start).normalized, Vector3.up);
            mid += perpendicular * distance * curveAmount;

            Vector3 prevPos = start;

            if (CanSpawn(pointA))
            { 
                ShapeInfo shapeInfo = Instantiate(shapePrefab, start, Quaternion.LookRotation((mid - start).normalized), this.transform).GetComponent<ShapeInfo>();
                shapeInfo.position = start;
                ShapePointContext.shapeNote = shapeInfo;
                pointA.hasShape = true;
                pointA.shapeInfo = shapeInfo;

                EventBus<FirstShapeEvent>.Raise(new FirstShapeEvent(shapeInfo));
            }
            else
            {
                ShapePointContext.shapeNote = pointA.shapeInfo;
            }

            float step = 1f / segmentCount;
            for (int i = 1; i <= segmentCount; i++)
            {
                float t = i * step;
                Vector3 pos = Bezier.GetQuadraticBezierPoint(t, start, mid, end);

                Vector3 dir = (pos - prevPos).normalized;
                Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

                if (Vector3.Distance(prevPos, pos) >= sideLength)
                {
                    ShapeInfo shapeInfo = Instantiate(shapePrefab, pos, rot, this.transform).GetComponent<ShapeInfo>();
                    shapeInfo.position = pos;
                    //MazeChainSpawnerManager.Instance.SpawnChain(this.transform, ShapePointContext.shapeNote.position, pos);
                    ShapePointContext.shapeNote.AddTarget(shapeInfo);
                    ShapePointContext.shapeNote = shapeInfo;
                    shapeInfo.shapeSO = WeightedRandom.Instance.GetWeightedRandom();
                    prevPos = pos;
                }
            }

            pointB.hasShape = true;
            pointB.shapeInfo = ShapePointContext.shapeNote;

            return UniTask.CompletedTask;
        }

        private bool CanSpawn(MazePoint mazePoint)
        {
            if (mazePoint != null && !mazePoint.hasShape)
            {
                return true;
            }

            return false;
        }
    }
}