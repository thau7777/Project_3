using Cysharp.Threading.Tasks;
using MyRule.CommandPattern;
using Turnbase;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace MyRule
{
    public class ClassView : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private EClass classType;
        [SerializeField] private CharacterStatsSO characterStatsSO;
        [SerializeField] private GameObject highlightObj;

        private void OnEnable()
        {
            
        }

        public void OnDeselect(BaseEventData eventData)
        {
            highlightObj.SetActive(false);
        }

        public void OnSelect(BaseEventData eventData)
        {
            highlightObj.SetActive(true);

            EventBus<CharacterModelEvent>.Raise(new CharacterModelEvent(classType));

            CharacterStatsManager.Instance.SetBase(characterStatsSO);
            CharacterStatsData data = CharacterStatsManager.Instance.GetCharacterStats();
            EventBus<CharacterStatsUpdatedEvent>.Raise(new CharacterStatsUpdatedEvent(data));

            SigilStorageManager.Instance.ResetStorage();

            RuneManger.Instance.SetStartRune(characterStatsSO.rune);
        }
    }
}