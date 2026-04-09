using Cysharp.Threading.Tasks;
using MyRule.CommandPattern;
using TMPro;
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
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI backStory;
        [SerializeField] private bool locking = true;

        private void Start()
        {
            button.onClick.AddListener(EnterPortal);
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

            backStory.text = characterStatsSO.backStory;
        }

        private void EnterPortal()
        {
            PortalManager.Instance.OnStartBtnClicked();
        }
    }
}