using MyRule.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyRule
{
    public class MazeGameplayManager : MonoBehaviour
    {
        private EventBinding<MazeGameplayEvent> _mazeGameplayEventBinding;

        private void OnEnable()
        {
            _mazeGameplayEventBinding = new EventBinding<MazeGameplayEvent>(OnMazeGameplayEvent);
            EventBus<MazeGameplayEvent>.Register(_mazeGameplayEventBinding);
        }

        private void OnDisable()
        {
            EventBus<MazeGameplayEvent>.Deregister(_mazeGameplayEventBinding);
        }

        private void OnMazeGameplayEvent(MazeGameplayEvent evt)
        {
            switch (evt.nodeType)
            {
                case NodeType.MinorEnemy:
                    break;
                case NodeType.Store:
                    EventBus<SwitchPanelEvent>.Raise(new SwitchPanelEvent(PanelType.Store));
                    break;
                case NodeType.Treasure:
                    break;
                case NodeType.RestSite:
                    break;
                case NodeType.Mystery:
                    break;
                case NodeType.Boss:
                    break;
                default:
                    Debug.Log("Unknown shape type");
                    break;
            }
        }
    }
}