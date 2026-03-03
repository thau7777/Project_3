using System;
using UnityEngine;

namespace MyRule
{
    public enum EClass
    {
        Swordman,
        Archer,
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
            foreach (var characterModel in characterModels)
            {
                if (characterModel.characterClass == e.characterClass)
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