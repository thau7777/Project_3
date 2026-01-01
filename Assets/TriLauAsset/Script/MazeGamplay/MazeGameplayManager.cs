using UnityEngine;

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
            switch (evt.shapeType)
            {
                case ShapeType.MiniGame:
                    Debug.Log("Enter Mini Event");
                    break;
                case ShapeType.Boss:
                    Debug.Log("Enter Boss Event");
                    break;
                case ShapeType.Sigil:
                    EventBus<SigilBoardEnterEvent>.Raise(new SigilBoardEnterEvent());
                    Debug.Log("Enter Sigil Event");
                    break;
                case ShapeType.Treasure:
                    Debug.Log("Enter Treasure Event");
                    break;
                case ShapeType.Creeeps:
                    Debug.Log("Enter Creeps Event");
                    break;
                case ShapeType.Recovery:
                    Debug.Log("Recovery");
                    break;
                case ShapeType.Shop:
                    Debug.Log("Enter Shop Event");
                    break;
                default:
                    Debug.Log("Unknown shape type");
                    break;
            }
        }
    }
}