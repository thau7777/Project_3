using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace MyRule.UI
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI currentHealthTxt;
        [SerializeField] private Material healthFill;

        private int prevHealth = 0;

        private EventBinding<CharacterUpdatedEvent> healthUpdateEvent;

        private void OnEnable()
        {
            healthUpdateEvent = new EventBinding<CharacterUpdatedEvent>(UpdateHealth);
            EventBus<CharacterUpdatedEvent>.Register(healthUpdateEvent);
        }

        private void OnDisable()
        {
            EventBus<CharacterUpdatedEvent>.Deregister(healthUpdateEvent);
        }

        private async void UpdateHealth(CharacterUpdatedEvent evt)
        {
            int currentHealth = evt.character.CharacterStatsData.BaseStatsData.CurrentHealth;
            int maxHealth = evt.character.CharacterStatsData.BaseStatsData.MaxHealth;
            float healthRate = evt.character.CharacterStatsData.BaseStatsData.GetHealthRate();

            float time = 0f;
            float duration = 0.5f;

            while (time < duration)
            {
                time += Time.deltaTime;

                int value = Mathf.RoundToInt(Mathf.Lerp(prevHealth, currentHealth, time / duration));
                float fillLevel = Mathf.Lerp(prevHealth, healthRate, time / duration);

                currentHealthTxt.text = value + "/" + maxHealth;
                healthFill.SetFloat("_FillLevel", fillLevel);

                await UniTask.Yield();
            }


            currentHealthTxt.text = currentHealth.ToString() + "/" + maxHealth.ToString();
            healthFill.SetFloat("_FillLevel", healthRate);

            prevHealth = evt.character.CharacterStatsData.BaseStatsData.CurrentHealth;
        }
    }
}