using Newtonsoft.Json;
using System;

namespace MyRule
{
    [Serializable]
    public class GameData
    {
        [JsonProperty] private LobbyData _lobby;
        [JsonProperty] private SigilCollectionData _sigilCollection;
        [JsonProperty] private HistoryData _history;
        [JsonProperty] private MatchData _matchData;

        [JsonIgnore] public LobbyData LobbyData => _lobby;
        [JsonIgnore] public SigilCollectionData SigilCollection => _sigilCollection;
        [JsonIgnore] public HistoryData History => _history;
        [JsonIgnore] public MatchData MatchData => _matchData;

        public GameData() 
        {
            this._lobby = new LobbyData();
            this._sigilCollection = new SigilCollectionData();
            this._history = new HistoryData();
        }

        public void SetSigilCollection(SigilCollectionData sigilCollection) => this._sigilCollection = sigilCollection;

        public void CreatNewMatch(int runeAmount, SigilsInMatchData sigilInMatch) => _matchData = new MatchData(runeAmount, sigilInMatch);

        public void SetMatch(MatchData matchData) => this._matchData = matchData;
    }
}