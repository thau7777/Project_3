using Newtonsoft.Json;
using System;

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
        [JsonProperty] private SigilPool _sigilPoolInMatch;
        [JsonProperty] private SigilStorageData _sigilStorageInMatch;
        [JsonProperty] private ItemStorageData _itemStorageInMatch;
        [JsonProperty] private int _enmiesDefeated;
        [JsonProperty] private int _nodesExplored;
        [JsonProperty] private CombatData _combatData;
        [JsonProperty] private MazeGameplayReward _reward;
        [JsonProperty] private WeatherData _weatherData;

        [JsonIgnore] public EMap MapType => _mapType;
        [JsonIgnore] public Loader.EScene Scene => _scene;
        [JsonIgnore] public bool IsNewMatch => _isNewMatch;
        [JsonIgnore] public EMatchResult Result => _result;
        [JsonIgnore] public CharacterData CharacterData => _characterData;
        [JsonIgnore] public int RuneInMatch => _runeInMatch;
        [JsonIgnore] public bool HasReceivedStartingSigil => _hasReceivedStartingSigil;
        [JsonIgnore] public SigilPool SigilPool => _sigilPoolInMatch;
        [JsonIgnore] public SigilStorageData SigilStorageInMatch => _sigilStorageInMatch;
        [JsonIgnore] public ItemStorageData ItemStorageInMatch => _itemStorageInMatch;
        [JsonIgnore] public int EnemiesDefeated => _enmiesDefeated;
        [JsonIgnore] public int NodesExplored => _nodesExplored;
        [JsonIgnore] public CombatData CombatData => _combatData;
        [JsonIgnore] public MazeGameplayReward Reward => _reward;
        [JsonIgnore] public WeatherData WeatherData => _weatherData;

        public MatchData(EMap eMap, CharacterData character, int runeAmount)
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
            _sigilPoolInMatch = new SigilPool();
            _sigilStorageInMatch = new SigilStorageData();
            _itemStorageInMatch = new ItemStorageData();
            _enmiesDefeated = 0;
            _nodesExplored = 0;
            _combatData = null;
            _reward = null;
            _weatherData = null;
        }

        public async void MoveToNextMap()
        {
            switch (_mapType)
            {
                case EMap.GreenLand:
                    _mapType = EMap.Desert;
                    _scene = Loader.EScene.DesertScene;
                    await Loader.LoadSceneWithLoading(Loader.EScene.DesertScene);
                    break;
                case EMap.Desert:
                    _mapType = EMap.IceLand;
                    _scene = Loader.EScene.IcelandScene;
                    await Loader.LoadSceneWithLoading(Loader.EScene.IcelandScene);
                    break;
                case EMap.IceLand:
                    break;
            }
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

        public void SetCombat(CombatData combatData) => this._combatData = combatData;

        public void SetReward(MazeGameplayReward reward) => this._reward = reward;

        public void SetWeather(WeatherData weatherData) => this._weatherData = weatherData;
    }
}