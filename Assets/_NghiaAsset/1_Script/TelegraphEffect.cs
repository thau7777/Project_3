using UnityEngine;

namespace Turnbase
{
    public class TelegraphEffect : MonoBehaviour
    {
        [Header("Settings")]
        public GameObject telegraphPrefab;
        public GameObject telegraphPrefab2;

        public Transform telegraphSpawnPoint;

        private GameObject currentEffect;

        public void Play(bool canParry)
        {
            if (telegraphSpawnPoint == null) return;

            if (currentEffect != null) Destroy(currentEffect);

            GameObject prefabToSpawn = canParry ? telegraphPrefab : telegraphPrefab2;

            if (prefabToSpawn != null)
            {
                currentEffect = Instantiate(prefabToSpawn, telegraphSpawnPoint.position, telegraphSpawnPoint.rotation);
                currentEffect.transform.SetParent(telegraphSpawnPoint);
            }
        }

        public void Stop()
        {
            if (currentEffect != null)
            {
                Destroy(currentEffect);
                currentEffect = null;
            }
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