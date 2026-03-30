using UnityEngine;
using TMPro;
using System.Collections;

namespace Turnbase
{
    public class ParryPopup : MonoBehaviour
    {
        public GameObject parryPopupPrefab;
        public GameObject avoidPopupPrefab;
        public Transform spawnPoint;

        public void ShowParryPopup(Character character, string message = "PARRIED")
        {
            Vector3 pos = spawnPoint != null ? spawnPoint.position : character.transform.position + Vector3.up * 2f;
            GameObject popup = Instantiate(parryPopupPrefab, pos, Quaternion.identity);

            StartCoroutine(FadeOutAndDestroy(popup, message));
        }

        public void ShowAvoidPopup(Character character)
        {
            Vector3 pos = spawnPoint != null ? spawnPoint.position : character.transform.position + Vector3.up * 2f;
            GameObject popup = Instantiate(avoidPopupPrefab, pos, Quaternion.identity);

            StartCoroutine(FadeOutAndDestroy(popup, "AVOID"));
        }

        private IEnumerator FadeOutAndDestroy(GameObject popup, string message)
        {
            TextMeshPro textComponent = popup.GetComponentInChildren<TextMeshPro>();
            if (textComponent == null)
            {
                Destroy(popup);
                yield break;
            }

            textComponent.text = message;

            float duration = 1.0f;
            float elapsed = 0f;

            Color originalColor = textComponent.color;
            Vector3 startPos = popup.transform.position;
            Vector3 targetPos = startPos + Vector3.up * 0.5f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float normalizedTime = elapsed / duration;

                textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f - normalizedTime);

                popup.transform.position = Vector3.Lerp(startPos, targetPos, normalizedTime);

                yield return null;
            }

            Destroy(popup);
        }
    }
}