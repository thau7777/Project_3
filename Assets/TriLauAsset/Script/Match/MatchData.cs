using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MyRule
{
    public enum EMatchResult
    {
        None,
        Win,
        Lose
    }

    [Serializable]
    public class MatchData
    {
        [JsonProperty] private EMatchResult _result;
        [JsonProperty] private int _runeInMatch;
        [JsonProperty] private bool _hasReceivedStartingSigil;
        [JsonProperty] private CharacterStatsData _characterStatsData;
        [JsonProperty] private SigilsInMatchData _sigilsInMatch;
        [JsonProperty] private SigilStorageData _sigilStorageInMatch;
        [JsonProperty] private ItemStorageData _itemStorageInMatch;

        [JsonIgnore] public EMatchResult Result => _result;
        [JsonIgnore] public CharacterStatsData CharacterStatsData => _characterStatsData;
        [JsonIgnore] public int RuneInMatch => _runeInMatch;
        [JsonIgnore] public bool HasReceivedStartingSigil => _hasReceivedStartingSigil;
        [JsonIgnore] public SigilsInMatchData SigilsInMatch => _sigilsInMatch;
        [JsonIgnore] public SigilStorageData SigilStorageInMatch => _sigilStorageInMatch;
        [JsonIgnore] public ItemStorageData ItemStorageInMatch => _itemStorageInMatch;

        public MatchData(CharacterStatsData characterStats, int runeAmount, SigilsInMatchData sigilsInMatch)
        {
            _result = EMatchResult.None;
            _characterStatsData = characterStats;
            _runeInMatch = runeAmount;
            _hasReceivedStartingSigil = false;
            _sigilsInMatch = sigilsInMatch;
            _sigilStorageInMatch = new SigilStorageData();
            _itemStorageInMatch = new ItemStorageData();
        }
        public void SetMatchResult(EMatchResult result) => this._result = result;

        public void SetCharacterStats(CharacterStatsData characterStats) => this._characterStatsData = characterStats;

        public void SetRuneInMatch(int runeAmount) => this._runeInMatch = runeAmount;

        public void SetHasRecivedStartingSigil(bool hasRecivedStartingSigil) => _hasReceivedStartingSigil= hasRecivedStartingSigil;

        public void SetSigilStorageInMatch(SigilStorageData sigilStorageData) => this._sigilStorageInMatch = sigilStorageData;
    
        public void SetItemStorageInMatch(ItemStorageData itemStorageData) => this._itemStorageInMatch = itemStorageData;
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