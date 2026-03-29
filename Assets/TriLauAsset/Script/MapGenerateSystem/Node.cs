using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace MyRule
{
    [Serializable]
    public class Node
    {
        [JsonProperty] private Vector2Int point;
        [JsonProperty] private List<Vector2Int> incoming = new List<Vector2Int>();
        [JsonProperty] private List<Vector2Int> outgoing = new List<Vector2Int>();
        [JsonProperty] [JsonConverter(typeof(StringEnumConverter))] private NodeType nodeType;
        [JsonProperty] private string blueprintName;
        [JsonProperty] private Vector2 position;

        [JsonIgnore] public Vector2Int Point
        {
            get { return point; }
            set { point = value; }
        }
        [JsonIgnore] public List<Vector2Int> Incoming
        {
            get { return incoming; }
            set { incoming = value; }
        }
        [JsonIgnore] public List <Vector2Int> Outgoing
        {
            get { return outgoing; }
            set { outgoing = value; }
        }
        [JsonIgnore] public NodeType NodeType
        {
            get { return nodeType; }
            set { nodeType = value; }
        }
        [JsonIgnore] public string BlueprintName
        {
            get { return blueprintName; }
            set { blueprintName = value; }
        }
        [JsonIgnore] public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Node(NodeType nodeType, string blueprintName, Vector2Int point)
        {
            this.nodeType = nodeType;
            this.blueprintName = blueprintName;
            this.point = point;
        }

        public void AddIncoming(Vector2Int p)
        {
            if (incoming.Any(element => element.Equals(p)))
                return;

            incoming.Add(p);
        }

        public void AddOutgoing(Vector2Int p)
        {
            if (outgoing.Any(element => element.Equals(p)))
                return;

            outgoing.Add(p);
        }

        public void RemoveIncoming(Vector2Int p)
        {
            incoming.RemoveAll(element => element.Equals(p));
        }

        public void RemoveOutgoing(Vector2Int p)
        {
            outgoing.RemoveAll(element => element.Equals(p));
        }

        public bool HasNoConnections()
        {
            return incoming.Count == 0 && outgoing.Count == 0;
        }
    }
}