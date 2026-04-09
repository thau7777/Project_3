using UnityEngine;


namespace MyRule
{
    public class LookAtCamera : MonoBehaviour
    {
        [SerializeField] private Transform obj;

        void Update()
        {
            obj.LookAt(transform.position + Camera.main.transform.forward);
        }
    }
}