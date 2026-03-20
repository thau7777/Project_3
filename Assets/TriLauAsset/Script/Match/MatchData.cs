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
        [JsonProperty] private EMap _mapType;
        [JsonProperty] private Loader.EScene _scene;
        [JsonProperty] private bool _isNewMatch;
        [JsonProperty] private EMatchResult _result;
        [JsonProperty] private int _runeInMatch;
        [JsonProperty] private bool _hasReceivedStartingSigil;
        [JsonProperty] private CharacterData _characterData;
        [JsonProperty] private SigilsInMatchData _sigilsInMatch;
        [JsonProperty] private SigilStorageData _sigilStorageInMatch;
        [JsonProperty] private ItemStorageData _itemStorageInMatch;
        [JsonProperty] private int _enmiesDefeated;
        [JsonProperty] private int _nodesExplored;

        [JsonIgnore] public EMap MapType => _mapType;
        [JsonIgnore] public Loader.EScene Scene => _scene;
        [JsonIgnore] public bool IsNewMatch => _isNewMatch;
        [JsonIgnore] public EMatchResult Result => _result;
        [JsonIgnore] public CharacterData CharacterData => _characterData;
        [JsonIgnore] public int RuneInMatch => _runeInMatch;
        [JsonIgnore] public bool HasReceivedStartingSigil => _hasReceivedStartingSigil;
        [JsonIgnore] public SigilsInMatchData SigilsInMatch => _sigilsInMatch;
        [JsonIgnore] public SigilStorageData SigilStorageInMatch => _sigilStorageInMatch;
        [JsonIgnore] public ItemStorageData ItemStorageInMatch => _itemStorageInMatch;
        [JsonIgnore] public int EnemiesDefeated => _enmiesDefeated;
        [JsonIgnore] public int NodesExplored => _nodesExplored;

        public MatchData(EMap eMap, CharacterData character, int runeAmount, SigilsInMatchData sigilsInMatch)
        {
            _mapType = eMap;

            switch (eMap)
            {
                case EMap.GreenLand:
                    _scene = Loader.EScene.GreenlandScene;
                    break;
                case EMap.Desert:
                    _scene = Loader.EScene.DesertScene;
                    break;
                case EMap.IceLand:
                    _scene = Loader.EScene.IcelandScene;
                    break;
            }

            _isNewMatch = true;
            _result = EMatchResult.None;
            _characterData = character;
            _runeInMatch = runeAmount;
            _hasReceivedStartingSigil = false;
            _sigilsInMatch = sigilsInMatch;
            _sigilStorageInMatch = new SigilStorageData();
            _itemStorageInMatch = new ItemStorageData();
            _enmiesDefeated = 0;
            _nodesExplored = 0;
        }

        public void SetMap(EMap eMap) => _mapType = eMap;

        public void SetIsNewMatch(bool isNewMatch) => _isNewMatch = isNewMatch;

        public void SetMatchResult(EMatchResult result) => this._result = result;

        public void SetCharacter(CharacterData character) => this._characterData = character;

        public void SetRuneInMatch(int runeAmount) => this._runeInMatch = runeAmount;

        public void SetHasRecivedStartingSigil(bool hasRecivedStartingSigil) => _hasReceivedStartingSigil= hasRecivedStartingSigil;

        public void SetSigilStorageInMatch(SigilStorageData sigilStorageData) => this._sigilStorageInMatch = sigilStorageData;
    
        public void SetItemStorageInMatch(ItemStorageData itemStorageData) => this._itemStorageInMatch = itemStorageData;
        
        public void IncreaseEnmiesDefeated() => _enmiesDefeated++;

        public void IncreaseNodesExplored() => _nodesExplored++;
    }
}