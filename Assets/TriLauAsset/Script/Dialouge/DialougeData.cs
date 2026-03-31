using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    [JsonObject]
    public class DialougeData
    {
        [JsonProperty] private Dictionary<string, object> _keyValuePairs;

        [JsonIgnore] public Dictionary<string, object> KeyValuePairs => _keyValuePairs;

        [JsonConstructor]
        public DialougeData()
        {
            _keyValuePairs = new Dictionary<string, object>();
        }
    }
}