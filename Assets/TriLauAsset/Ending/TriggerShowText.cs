using TMPro;
using UnityEngine;

namespace MyRule
{
    public class TriggerShowText : MonoBehaviour
    {
        [SerializeField] private Transform textPoint;
        [SerializeField] private TextMeshPro text;
        [SerializeField] private float fadeSpeed = 5f;

        private float targetAlpha = 0f;

        private void Start()
        {
            SetAlpha(0f);
        }

        private void Update()
        {
            float current = text.alpha;
            float newAlpha = Mathf.MoveTowards(current, targetAlpha, fadeSpeed * Time.deltaTime);
            SetAlpha(newAlpha);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                text.transform.position = textPoint.position;
                targetAlpha = 1f;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                targetAlpha = 0f;
            }
        }

        private void SetAlpha(float a)
        {
            Color c = text.color;
            c.a = a;
            text.color = c;
        }
    }
}