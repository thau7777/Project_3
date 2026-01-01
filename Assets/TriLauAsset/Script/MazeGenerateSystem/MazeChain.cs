using UnityEngine;

namespace MyRule
{
    public class MazeChain : MonoBehaviour
    {
        public Transform startPoint;
        public Transform midPoint1;
        public Transform midPoint2;
        public Transform endPoint;
        
        public void Initialize(Vector3 start, Vector3 end)
        {
            startPoint.position = start;
            endPoint.position = end;

            Vector3 mid = (start + end) / 2f;

            Vector3 pos = Bezier.GetQuadraticBezierPoint(1, start, mid, end);

            midPoint1.position = pos;
            midPoint2.position = pos;
        }
    }
}
