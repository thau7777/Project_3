using MyRule.Event;
using UnityEngine;

namespace MyRule
{
    public class DialougeTrigger : MonoBehaviour
    {
        [SerializeField] private string dialougeName;

        public void Trigger()
        {
            if (dialougeName != "")
            {
                EventBus<EnterDialogueEvent>.Raise(new EnterDialogueEvent(dialougeName));
                Debug.Log("trigger dialogue: " + dialougeName);
            }
        }
    }
}