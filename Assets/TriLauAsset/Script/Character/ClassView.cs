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
        [SerializeField] private CharacterSO characterStatsSO;
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

            CharacterManager.Instance.SetBase(characterStatsSO);
            CharacterData data = CharacterManager.Instance.GetCharacterStats();
            EventBus<CharacterUpdatedEvent>.Raise(new CharacterUpdatedEvent(data));

            RuneManger.Instance.SetStartRune(characterStatsSO.startRune);
        }
    }
}