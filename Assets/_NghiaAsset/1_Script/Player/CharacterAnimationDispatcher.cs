using UnityEngine;
using System;

namespace Turnbase
{
    public class CharacterAnimationDispatcher : MonoBehaviour
    {
        private Action onSpawnAction;

        public void SetSpawnCallback(Action callback)
        {
            onSpawnAction = callback;
        }

        public void TriggerSpawn()
        {
            if (onSpawnAction != null)
            {
                onSpawnAction.Invoke();
                onSpawnAction = null; 
            }
        }
    }
}