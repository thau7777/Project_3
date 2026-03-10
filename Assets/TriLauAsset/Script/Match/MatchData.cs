using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MyRule
{
    [Serializable]
    public class MatchData
    {
        [JsonProperty] private int _runeInMatch;
        [JsonProperty] private SigilsInMatchData _sigilsInMatch;
        [JsonProperty] private SigilStorageData _sigilStorageInMatch;
        [JsonProperty] private ItemStorageData _itemStorgeInMatch;

        [JsonIgnore] public int RuneInMatch => _runeInMatch;
        [JsonIgnore] public SigilsInMatchData SigilsInMatch => _sigilsInMatch;
        [JsonIgnore] public SigilStorageData SigilStorageInMatch => _sigilStorageInMatch;
        [JsonIgnore] public ItemStorageData ItemStorageInMatch => _itemStorgeInMatch;

        public MatchData(int runeAmount, SigilsInMatchData sigilsInMatch)
        {
            _runeInMatch = runeAmount;
            _sigilsInMatch = sigilsInMatch;
            _sigilStorageInMatch = new SigilStorageData();
            _itemStorgeInMatch = new ItemStorageData();
        }

        public void SetRuneInMatch(int runeAmount) => this._runeInMatch = runeAmount;
    }

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