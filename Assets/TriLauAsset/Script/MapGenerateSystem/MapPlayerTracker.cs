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
    public class MapPlayerTracker : Singleton<MapPlayerTracker>
    {
        public bool lockAfterSelecting = false;
        public float enterNodeDelay = 1f;
        public MapManager mapManager;
        public MapView view;

        public bool Locked { get; set; }

        private int locking = 0;

        public bool LockOOG { get; set; }

        public void LockMapTracker() => locking++;
        public void UnlockMapTracker() => locking--;

        public void SelectNode(MapNode mapNode)
        {
            if (locking > 0) return;

            if (Locked) return;

            // Debug.Log("Selected node: " + mapNode.Node.point);

            if (mapManager.CurrentMap.Path.Count == 0)
            {
                if (mapNode.Node.Point.y == 0)
                    SendPlayerToNode(mapNode);
                else
                    PlayWarningThatNodeCannotBeAccessed();
            }
            else
            {
                Vector2Int currentPoint = mapManager.CurrentMap.Path[mapManager.CurrentMap.Path.Count - 1];
                Node currentNode = mapManager.CurrentMap.GetNode(currentPoint);

                if (currentNode != null && currentNode.Outgoing.Any(point => point.Equals(mapNode.Node.Point)))
                    SendPlayerToNode(mapNode);
                else
                    PlayWarningThatNodeCannotBeAccessed();
            }
        }

        private void SendPlayerToNode(MapNode mapNode)
        {
            Locked = lockAfterSelecting;
            mapManager.CurrentMap.Path.Add(mapNode.Node.Point);
            view.SetAttainableNodes();
            view.SetLineColors();
            mapNode.ShowSwirlAnimation();

            DOTween.Sequence().AppendInterval(enterNodeDelay).OnComplete(() => EnterNode(mapNode));
        }

        private void EnterNode(MapNode mapNode)
        {
            Debug.Log("Entering node: " + mapNode.Node.BlueprintName + " of type: " + mapNode.Node.NodeType + mapNode.transform.position);

            //EventBus<MazeMoveEvent>.Raise(new MazeMoveEvent(mapNode.transform, mapNode.Node.nodeType));

            MatchManager.Instance.MatchData.IncreaseStep();

            if (LockOOG) return;

            switch (mapNode.Node.NodeType)
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