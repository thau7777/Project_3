using Newtonsoft.Json;
using System;

namespace MyRule
{
    [JsonObject]
    public class HistoryData
    {
        [JsonProperty] private HistotyMatchData[] _matchs;

        [JsonIgnore] public HistotyMatchData[] Matchs => _matchs;

        [JsonConstructor]
        public HistoryData()
        {
            _matchs = new HistotyMatchData[5];
        }

        private int GetEmpty()
        {
            for (int i = 0; i < _matchs.Length; i++)
            {
                if (_matchs[i] == null) return i;
            }
            return -1;
        }

        public void AddMatch(MatchData match)
        {
            HistotyMatchData historyMatchData = new HistotyMatchData(match.Result, match.TimePlayed, match.CharacterData.CharacterClass, match.EnemiesDefeated, match.NodesExplored, match.DamageInflicted, match.DamagePrevented, match.SigilStorageInMatch);
            int index = GetEmpty();

            if (index == -1)
            {
                for (int i = 0; i < _matchs.Length - 1; i++)
                {
                    _matchs[i] = _matchs[i + 1];
                }
                _matchs[_matchs.Length - 1] = historyMatchData;
                return;
            }
            else
            {
                _matchs[index] = historyMatchData;
            }
        }
    }

    [JsonObject]
    public class HistotyMatchData
    {
        [JsonProperty] private EMatchResult result;
        [JsonProperty] private float timePlayed;
        [JsonProperty] private EClass characterClass;
        [JsonProperty] private int enmiesDefeated;
        [JsonProperty] private int nodesExplored;
        [JsonProperty] private int damageInflicted;
        [JsonProperty] private int damagePrevented;
        [JsonProperty] private SigilStorageData sigilStorageInMatch;

        [JsonIgnore] public EMatchResult Result => result;
        [JsonIgnore] public float TimePlayed => timePlayed;
        [JsonIgnore] public EClass CharacterClass => characterClass;
        [JsonIgnore] public int EnemiesDefeated => enmiesDefeated;
        [JsonIgnore] public int NodesExplored => nodesExplored;
        [JsonIgnore] public int DamageInflicted => damageInflicted;
        [JsonIgnore] public int DamagePrevented => damagePrevented;
        [JsonIgnore] public SigilStorageData SigilStorageInMatch => sigilStorageInMatch;

        [JsonConstructor]
        public HistotyMatchData(EMatchResult result, float timePlayed, EClass characterClass, int enmiesDefeated, int nodesExplored, int damageInflicted, int damagePrevented, SigilStorageData sigilStorageInMatch)
        {
            this.result = result;
            this.timePlayed = timePlayed;
            this.characterClass = characterClass;
            this.enmiesDefeated = enmiesDefeated;
            this.nodesExplored = nodesExplored;
            this.damageInflicted = damageInflicted;
            this.damagePrevented = damagePrevented;
            this.sigilStorageInMatch = sigilStorageInMatch;
        }
    }
}