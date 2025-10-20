using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DamageAllCommand : ICommand
{
    private Character user;
    private Skill skill;
    private BattleManager battleManager;

    public DamageAllCommand(Character user, Skill skill, BattleManager battleManager)
    {
        this.user = user;
        this.skill = skill;
        this.battleManager = battleManager;
    }

    public IEnumerator Execute()
    {
        Debug.Log($"{user.name} dùng skill AOE {skill.skillName}");

        user.animator.Play("Cast");
        yield return new WaitForSeconds(1.5f);


        const float attackMultiplier = 0.5f;

        int offensiveStat = user.stats.attack;

        switch (skill.elementType)
        {
            case ElementType.Magical:
            case ElementType.Fire:
            case ElementType.Ice:
            case ElementType.Poison:
            case ElementType.Lightning:
            case ElementType.Holy:
            case ElementType.Dark:
                offensiveStat = user.stats.magicAttack;
                break;

            case ElementType.Physical:
            case ElementType.None:
            default:
                offensiveStat = user.stats.attack;
                break;
        }

        int rawDamage = skill.damage + Mathf.RoundToInt(offensiveStat * attackMultiplier);



        List<Character> allTargets;
        if (skill.targetType == SkillTargetType.Enemies)
        {
            allTargets = battleManager.allCombatants.FindAll(c => c != null && !c.isPlayer && c.isAlive);
        }
        else
        {
            allTargets = battleManager.allCombatants.FindAll(c => c != null && c.isPlayer && c.isAlive);
        }

        foreach (Character aoeTarget in allTargets)
        {
            int defensiveStat = aoeTarget.stats.defense;
            if (skill.elementType != ElementType.Physical && skill.elementType != ElementType.None)
            {
                defensiveStat = aoeTarget.stats.magicDefense;
            }

            float damageMultiplier = 100f / (defensiveStat + 100f);

            int finalDamage = Mathf.RoundToInt(rawDamage * damageMultiplier);

            if (rawDamage > 0)
            {
                finalDamage = Mathf.Max(1, finalDamage);
            }

            aoeTarget.TakeDamage(finalDamage);

            SpawnImpactEffect(aoeTarget.transform.position);
        }

        yield return new WaitForSeconds(0.5f);

        if (battleManager != null)
        {
            battleManager.EndTurn(user);
        }
    }

    private void SpawnImpactEffect(Vector3 position)
    {
        GameObject effectToSpawn = skill.impactVFXPrefab;

        if (effectToSpawn != null)
        {
            GameObject effectInstance = GameObject.Instantiate(effectToSpawn, position, Quaternion.identity);

            GameObject.Destroy(effectInstance, 3f);
        }
        else
        {
            Debug.LogWarning($"Thiếu Prefab Impact VFX cho kỹ năng: {skill.skillName}");
        }
    }
}