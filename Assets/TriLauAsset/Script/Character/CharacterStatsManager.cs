using Cysharp.Threading.Tasks;
using UnityEngine;


namespace MyRule
{
    public class CharacterStatsManager : PersistentSingleton<CharacterStatsManager>, IGameData
    {
        private CharacterStatsData characterStats;

        private void OnEnable()
        {
            GameSystemManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            GameSystemManager.Instance.Unregister(this);
        }

        public void SetBase(CharacterStatsSO stats)
        {
            characterStats.SetCharacterClass(stats.characterClass);

            AttributesData attributesData = new AttributesData(stats.vigor, stats.mind, stats.endurance, stats.strength, stats.dexterity, stats.intelligence, stats.faith, stats.arcane);
            characterStats.SetAttributesData(attributesData);

            BaseStatsData baseData = new BaseStatsData(stats.hp, stats.fp, stats.stamina, stats.speed, stats.critChance, stats.critMult);
            characterStats.SetBaseStatsData(baseData);

            DamageData damageData = new DamageData(stats.attackDmg, stats.magicDmg, stats.fireDmg, stats.lightningDmg, stats.holyDmg, stats.darkDmg, stats.frostDmg, stats.waterDmg, stats.poisonDmg);
            characterStats.SetDamge(damageData);

            DefenseData defenseData = new DefenseData(stats.phyDef, stats.magicDef, stats.fireDef, stats.lightningDef, stats.holyDef, stats.darkDef, stats.frostDef, stats.waterDef, stats.poisonDef);
            characterStats.SetDefense(defenseData);
        }

        public CharacterStatsData GetCharacterStats() => characterStats;

        public EClass GetCharacterClass() => characterStats.CharacterClass;

        public void UpdateSigilStats(SigilSO sigilSO)
        {
            
        }

        public UniTask LoadData(GameData data)
        {
            characterStats = new CharacterStatsData();
            
            if (data.MatchData?.CharacterStatsData != null)
            {
                characterStats = data.MatchData.CharacterStatsData;
            }
            EventBus<CharacterStatsUpdatedEvent>.Raise(new CharacterStatsUpdatedEvent(characterStats));
            
            return UniTask.CompletedTask;
        }

        public void SaveData(GameData data)
        {
            if (data.MatchData != null)
            {
                data.MatchData.SetCharacterStats(characterStats);
            }
        }
    }
}