using System;
using UnityEngine;

namespace MyRule
{
    public enum EClass
    {
        Swordman,
        Archer,
        Mage,
        Summoner,
        Base
    }

    public class CharacterModelManager : MonoBehaviour
    {
        [SerializeField] private CharacterModelInfo[] characterModels;

        private EventBinding<CharacterModelEvent> _modelEventBinding;

        private void OnEnable()
        {
            _modelEventBinding = new EventBinding<CharacterModelEvent>(OnCharacterModelEvent);
            EventBus<CharacterModelEvent>.Register(_modelEventBinding);
        }

        private void OnDisable()
        {
            EventBus<CharacterModelEvent>.Deregister(_modelEventBinding);
        }

        private void OnCharacterModelEvent(CharacterModelEvent e)
        {
            SetCurrentClass(e.characterClass);
        }

        private void SetCurrentClass(EClass cClass)
        {
            foreach (var characterModel in characterModels)
            {
                if (characterModel.characterClass == cClass)
                {
                    characterModel.model.SetActive(true);
                }
                else
                {
                    characterModel.model.SetActive(false);
                }
            }
        }
    }

    [Serializable]
    public class CharacterModelInfo
    {
        public EClass characterClass;
        public GameObject model;
    }
}