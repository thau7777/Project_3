using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        int calculatedDamage = skill.damage + Mathf.RoundToInt(user.stats.attack * attackMultiplier);


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
            aoeTarget.TakeDamage(calculatedDamage);

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