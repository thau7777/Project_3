using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MyRule
{
    [Serializable]
    public class GameData
    {
        [JsonProperty] private LobbyData _lobby;
        [JsonProperty] private SigilCollectionData _sigilCollection;
        [JsonProperty] private HistoryData _history;
        [JsonProperty] private List<AchievementData> _achievements;
        [JsonProperty] private MatchData _matchData;

        [JsonIgnore] public LobbyData LobbyData => _lobby;
        [JsonIgnore] public SigilCollectionData SigilCollection => _sigilCollection;
        [JsonIgnore] public HistoryData History => _history;
        [JsonIgnore] public List<AchievementData> Achievements => _achievements;
        [JsonIgnore] public MatchData MatchData => _matchData;

        public GameData() 
        {
            this._lobby = new LobbyData();
            this._sigilCollection = new SigilCollectionData();
            this._history = new HistoryData();
            this._achievements = new List<AchievementData>();
            this._matchData = null;
        }

        public void SetHistory(HistoryData historyData) => this._history = historyData; 

        public void SetAchivements(List<AchievementData> achievements) => this._achievements = achievements;

        public void SetSigilCollection(SigilCollectionData sigilCollection) => this._sigilCollection = sigilCollection;

        public MatchData CreateNewMatch(EMap mapType, CharacterData characterStatsData, int runeAmount) => _matchData = new MatchData(mapType, characterStatsData, runeAmount);

        public void SetMatch(MatchData matchData) => this._matchData = matchData;
    }
}