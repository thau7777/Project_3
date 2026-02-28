using UnityEngine;


namespace MyRule
{
    public class CharacterStatsManager : PersistentSingleton<CharacterStatsManager>
    {
        [SerializeField] CharacterStatsSO characterStats;

        public void SetBase(CharacterStatsSO stats)
        {
            characterStats.virgor = stats.virgor;
            characterStats.mind = stats.mind;
            characterStats.endurance = stats.endurance;
            characterStats.strength = stats.strength;
            characterStats.dexterity = stats.dexterity;
            characterStats.intelligence = stats.intelligence;
            characterStats.faith = stats.faith;
            characterStats.arcane = stats.arcane;

            characterStats.hp = stats.hp;
            characterStats.fp = stats.fp;
            characterStats.stamina = stats.stamina;

            characterStats.physicalDmg = stats.physicalDmg;
            characterStats.magicDmg = stats.magicDmg;
            characterStats.critChance = stats.critChance;
            characterStats.critMult = stats.critMult;
        }

        public CharacterStatsSO GetCharacterStats()
        {
            return characterStats;
        }

        public void UpdateSigilStats(SigilSO sigilSO)
        {
            characterStats.strength += sigilSO.str;
            characterStats.dexterity += sigilSO.dex;
            characterStats.intelligence += sigilSO.intel;
            characterStats.faith += sigilSO.faith;
            characterStats.arcane += sigilSO.arcane;
            
            characterStats.hp += sigilSO.health;
            characterStats.def += sigilSO.def;
            characterStats.resRate += sigilSO.resRate;
            characterStats.attackSpeed += sigilSO.attackSpeed;

            characterStats.physicalDmg += sigilSO.phys;
            characterStats.magicDmg += sigilSO.mag;
            characterStats.critChance += sigilSO.critChance;
            characterStats.critMult += sigilSO.critMult;
        }
    }
}