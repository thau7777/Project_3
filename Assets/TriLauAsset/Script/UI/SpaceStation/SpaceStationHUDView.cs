using Cysharp.Threading.Tasks;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace MyRule.UI
{
    public class SpaceStationHUDView : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;

        [SerializeField] private string shipLabelName = "ShipName";
        [SerializeField] private string shipName = "AETHERION";

        [SerializeField] private float charRevealInterval = 1.2f;
        [SerializeField] private float scrambleSpeed = 0.03f;

        private Label label;
        private int revealedCount = 0;
        private float revealTimer;
        private float scrambleTimer;
        private bool playing;

        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private void Awake()
        {
            label = uiDocument.rootVisualElement.Q<Label>(shipLabelName);
        }

        private async void Start()
        {
            label.style.display = DisplayStyle.None;

            await UniTask.Delay(10000);

            label.style.display = DisplayStyle.Flex;

            Play();
        }

        public void Play()
        {
            revealedCount = 0;
            revealTimer = 0f;
            scrambleTimer = 0f;
            playing = true;
        }

        private void Update()
        {
            if (!playing) return;

            revealTimer += Time.deltaTime;
            scrambleTimer += Time.deltaTime;

            // Mỗi interval → mở thêm 1 chữ
            if (revealTimer >= charRevealInterval)
            {
                revealTimer = 0f;
                revealedCount++;

                if (revealedCount > shipName.Length)
                {
                    label.text = shipName;
                    playing = false;
                    return;
                }
            }

            // Update chữ random
            if (scrambleTimer >= scrambleSpeed)
            {
                scrambleTimer = 0f;
                label.text = GenerateText();
            }
        }

        private string GenerateText()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < revealedCount; i++)
            {
                sb.Append(shipName[i]);
            }

            if (revealedCount < shipName.Length)
            {
                sb.Append(chars[Random.Range(0, chars.Length)]);
            }

            return sb.ToString();
        }
    }
}