using MyRule;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MyRule
{
    [Serializable]
    public class HistoryData
    {
        [JsonProperty] public List<MatchData> _matchs;

        [JsonIgnore] public List<MatchData> Matchs => _matchs;

        public HistoryData()
        {
            _matchs = new List<MatchData>();
        }

        public void AddMatch(MatchData match) => _matchs.Add(match);
    }
}