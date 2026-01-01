using UnityEngine;

namespace MyRule
{
    public static class Bezier
    {
        public static Vector3 GetQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            return (u * u * p0) + (2 * u * t * p1) + (t * t * p2);
        }
    }
}
