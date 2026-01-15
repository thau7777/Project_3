using UnityEngine;

namespace Turnbase
{
    public class TelegraphEffect : MonoBehaviour
    {
        [Header("Settings")]
        public GameObject telegraphPrefab;  
        public Transform telegraphSpawnPoint; 

        private GameObject currentEffect;

        public void Play(float duration)
        {
            if (telegraphPrefab == null || telegraphSpawnPoint == null) return;

            currentEffect = Instantiate(telegraphPrefab, telegraphSpawnPoint.position, telegraphSpawnPoint.rotation);

            currentEffect.transform.SetParent(telegraphSpawnPoint);

            Destroy(currentEffect, duration);
        }

        private void Update()
        {
            if (currentEffect != null && Camera.main != null)
            {
                currentEffect.transform.LookAt(currentEffect.transform.position + Camera.main.transform.rotation * Vector3.forward,
                                               Camera.main.transform.rotation * Vector3.up);
            }
        }
    }
}