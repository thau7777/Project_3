using Newtonsoft.Json;
using System;

namespace MyRule
{
    [Serializable]
    public class HistoryData
    {
        [JsonProperty] private MatchData[] _matchs;

        [JsonIgnore] public MatchData[] Matchs => _matchs;

        public HistoryData()
        {
            _matchs = new MatchData[5];
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
            int index = GetEmpty();

            if (index == -1)
            {
                for (int i = 0; i < _matchs.Length - 1; i++)
                {
                    _matchs[i] = _matchs[i + 1];
                }
                _matchs[_matchs.Length - 1] = match;
                return;
            }
            else
            {
                _matchs[index] = match;
            }
        }
    }
}