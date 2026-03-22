using UnityEngine;
using TMPro;

namespace Turnbase
{
    public class ParryPopup : MonoBehaviour
    {
        public GameObject popupPrefab;

        public Vector3 spawnOffset = new Vector3(0.7f, 2f, 1f);

        public void ShowParryPopup(Character character)
        {
            Vector3 spawnPosition = character.transform.position + spawnOffset;

            GameObject popupInstance = Instantiate(popupPrefab, spawnPosition, Quaternion.identity);

            TextMeshPro textComponent = popupInstance.GetComponentInChildren<TextMeshPro>();

            if (textComponent != null)
            {
                textComponent.text = "PARRY";
            }

            Destroy(popupInstance, 1.0f);
        }
    }
}