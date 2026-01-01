using UnityEngine;

namespace MyRule
{
    public class MazePoint : MonoBehaviour
    {
        public bool hasShape = false;
        public Transform point;
        public ShapeInfo shapeInfo;

        private void Start()
        {
            if (point == null)
            {
                point = this.transform;
            }
        }
    }
}
