using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace MyRule
{
    [Serializable]
    public class Map
    {
        [JsonProperty] private List<Node> nodes;
        [JsonProperty] private List<Vector2Int> path;
        [JsonProperty] private string bossNodeName;
        [JsonProperty] private string configName;

        [JsonIgnore] public List<Node> Nodes => nodes;
        [JsonIgnore] public List<Vector2Int> Path => path;
        [JsonIgnore] public string BossNodeName => bossNodeName;
        [JsonIgnore] public string ConfigName => configName;

        public Map(string configName, string bossNodeName, List<Node> nodes, List<Vector2Int> path)
        {
            this.configName = configName;
            this.bossNodeName = bossNodeName;
            this.nodes = nodes;
            this.path = path;
        }

        public Node GetBossNode()
        {
            return nodes.FirstOrDefault(n => n.NodeType == NodeType.Boss);
        }

        public float DistanceBetweenFirstAndLastLayers()
        {
            Node bossNode = GetBossNode();
            Node firstLayerNode = nodes.FirstOrDefault(n => n.Point.y == 0);

            if (bossNode == null || firstLayerNode == null)
                return 0f;

            return bossNode.Position.y - firstLayerNode.Position.y;
        }

        public Node GetNode(Vector2Int point)
        {
            return nodes.FirstOrDefault(n => n.Point.Equals(point));
        }
    }
}