using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace MyRule.UI
{
    public class UILoading : MonoBehaviour
    {
        [SerializeField] private RectTransform circle;
        [SerializeField] private float speed = 1f;
        [SerializeField] private TextMeshProUGUI text;

        private void Start()
        {
            RunText();
        }

        private void FixedUpdate()
        {
            circle.Rotate(0, 0, -(speed * Time.fixedDeltaTime));
        }

        private async void RunText()
        {
            float t = 0;

            while (true)
            {
                string dot = t == 3 ? "..." : t == 2 ? ".." : t == 1 ? "." : "";
                text.text = "Loading" + dot;
                if (t == 3) t = 0;
                else t++;
                await UniTask.Delay(500);
            }
        }
    }
}