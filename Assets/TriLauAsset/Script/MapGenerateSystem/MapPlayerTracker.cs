using System;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyRule.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyRule
{
    public class MapPlayerTracker : MonoBehaviour
    {
        public bool lockAfterSelecting = false;
        public float enterNodeDelay = 1f;
        public MapManager mapManager;
        public MapView view;

        public static MapPlayerTracker Instance;

        public bool Locked { get; set; }

        private void Awake()
        {
            Instance = this;
        }

        public void SelectNode(MapNode mapNode)
        {
            if (Locked) return;

            // Debug.Log("Selected node: " + mapNode.Node.point);

            if (mapManager.CurrentMap.path.Count == 0)
            {
                if (mapNode.Node.point.y == 0)
                    SendPlayerToNode(mapNode);
                else
                    PlayWarningThatNodeCannotBeAccessed();
            }
            else
            {
                Vector2Int currentPoint = mapManager.CurrentMap.path[mapManager.CurrentMap.path.Count - 1];
                Node currentNode = mapManager.CurrentMap.GetNode(currentPoint);

                if (currentNode != null && currentNode.outgoing.Any(point => point.Equals(mapNode.Node.point)))
                    SendPlayerToNode(mapNode);
                else
                    PlayWarningThatNodeCannotBeAccessed();
            }
        }

        private void SendPlayerToNode(MapNode mapNode)
        {
            Locked = lockAfterSelecting;
            mapManager.CurrentMap.path.Add(mapNode.Node.point);
            mapManager.SaveMap();
            view.SetAttainableNodes();
            view.SetLineColors();
            mapNode.ShowSwirlAnimation();

            DOTween.Sequence().AppendInterval(enterNodeDelay).OnComplete(() => EnterNode(mapNode));
        }

        private static async void EnterNode(MapNode mapNode)
        {
            Debug.Log("Entering node: " + mapNode.Node.blueprintName + " of type: " + mapNode.Node.nodeType + mapNode.transform.position);

            //EventBus<MazeMoveEvent>.Raise(new MazeMoveEvent(mapNode.transform, mapNode.Node.nodeType));

            switch (mapNode.Node.nodeType)
            {
                case NodeType.MinorEnemy:
                    {
                        CombatManager.Instance.CreateCombat();
                        break;
                    }
                case NodeType.RestSite:
                    CharacterManager.Instance.IncreaseHealth(60);
                    break;
                case NodeType.Treasure:
                    break;
                case NodeType.Store:
                    NPCManager.Instance.TriggetStore();
                    break;
                case NodeType.Boss:
                    CombatManager.Instance.CreateBossFighting();
                    break;
                case NodeType.Mystery:
                    NPCManager.Instance.RandomNPC();
                    break;
                default:
                    Debug.Log("Ko co j");
                    break;
            }
        }

        private void PlayWarningThatNodeCannotBeAccessed()
        {
            Debug.Log("Selected node cannot be accessed");
        }
    }
}