using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    public class ShapeInfo : MonoBehaviour
    {
        public Vector3 position;
        public List<ShapeInfo> pointsTarget = new List<ShapeInfo>();
        public SpriteRenderer icon;
        public ShapeSO shapeSO;

        private void Start()
        {
            SnapToGround();

            if (shapeSO != null)
            {
                icon.sprite = shapeSO.shapeIcon;
            }
            else
            {
                icon.sprite = null;
            }
        }

        private void SnapToGround()
        {
            Vector3 origin = transform.position + Vector3.up * 10f;

            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 10f * 10, LayerMask.GetMask("Ground")))
            {
                transform.position = hit.point;

                transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            }
            else
            {
                Debug.LogWarning($"{gameObject.name}: Không hit terrain!");
            }
        }

        public void AddTarget(ShapeInfo target)
        {
            pointsTarget.Add(target);
        }

        public ShapeInfo GetTarget(int index)
        {
            return pointsTarget[index];
        }
    }
}
