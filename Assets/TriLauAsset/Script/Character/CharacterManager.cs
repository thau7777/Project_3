using Cysharp.Threading.Tasks;
using UnityEngine;


namespace MyRule
{
    public class CharacterManager : PersistentSingleton<CharacterManager>, IGameData
    {
        private CharacterData character;

        private void OnEnable()
        {
            GameSystemManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            GameSystemManager.Instance.Unregister(this);
        }

        public void SetBase(CharacterSO characterSO)
        {
            AttributesData attributesData = new AttributesData(characterSO.vigor, characterSO.mind, characterSO.endurance, characterSO.strength, characterSO.dexterity, characterSO.intelligence, characterSO.faith, characterSO.arcane);
            
            BaseStatsData baseData = new BaseStatsData(characterSO.hp, characterSO.fp, characterSO.stamina, characterSO.speed, characterSO.critChance, characterSO.critMult);
            
            DamageData damageData = new DamageData(characterSO.attackDmg, characterSO.magicDmg, characterSO.fireDmg, characterSO.lightningDmg, characterSO.holyDmg, characterSO.darkDmg, characterSO.frostDmg, characterSO.waterDmg, characterSO.poisonDmg);
            
            DefenseData defenseData = new DefenseData(characterSO.phyDef, characterSO.magicDef, characterSO.fireDef, characterSO.lightningDef, characterSO.holyDef, characterSO.darkDef, characterSO.frostDef, characterSO.waterDef, characterSO.poisonDef);

            CharacterStatsData characterStatsData = new CharacterStatsData(attributesData, baseData, damageData, defenseData);

            character = new CharacterData(characterSO.name, characterSO.backStory, characterSO.characterClass, characterStatsData);
        }

        public CharacterData GetCharacterStats() => character;

        public EClass GetCharacterClass() => character.CharacterClass;

        public void UpdateSigilStats(SigilSO sigilSO)
        {
            
        }

        public UniTask LoadData(GameData data)
        {
            character = new CharacterData();
            
            if (data.MatchData?.CharacterData != null)
            {
                character = data.MatchData.CharacterData;
            }
            EventBus<CharacterUpdatedEvent>.Raise(new CharacterUpdatedEvent(character));
            
            return UniTask.CompletedTask;
        }

        public void SaveData(GameData data)
        {
            if (data.MatchData != null)
            {
                data.MatchData.SetCharacter(character);
            }
        }
    }
}