using TMPro;
using UnityEngine;

namespace MyRule.UI
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI currentHealth;
        [SerializeField] private Material healthFill;

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

        private void UpdateHealth(CharacterUpdatedEvent evt)
        {
            currentHealth.text = evt.character.CharacterStatsData.BaseStatsData.CurrentHealth + "/" + 
                                evt.character.CharacterStatsData.BaseStatsData.MaxHealth;
            float healthRate = evt.character.CharacterStatsData.BaseStatsData.GetHealthRate();
            Debug.Log(healthRate);
            healthFill.SetFloat("_FillLevel", healthRate);
        }
    }
}