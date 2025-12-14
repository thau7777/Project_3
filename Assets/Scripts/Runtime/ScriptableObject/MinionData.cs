using UnityEngine;

[CreateAssetMenu(fileName = "New Minion Data", menuName = "Scriptable Objects/TopDownMinionData")]
public class MinionData : ScriptableObject
{
    [field: SerializeField] public MinionsManager.MinionKind Kind { get; set; }
    [field: SerializeField] public GameObject MinionPrefab { get; set; }
    [field: SerializeField, ReadOnly] public int MaxHealth { get; private set; } = 100;
    [field: SerializeField, ReadOnly] public int MaxMana { get; private set; } = 100;
    [field: SerializeField, ReadOnly] public int BaseAttackDamage { get; private set; } = 20;
    [field: SerializeField, ReadOnly] public float ReviveTime { get; private set; } = 10;
    [field: SerializeField, ReadOnly] public float ReviveElapsedTime { get; private set; } = 0;
    public bool IsDead => ReviveElapsedTime > 0;

    public void SetMaxHealth(int maxHealth) => MaxHealth = maxHealth;
    public void SetMaxMana(int maxMana) => MaxMana = maxMana;
    public void SetBaseAttackDamage(int baseAttackDamage) => BaseAttackDamage = baseAttackDamage;
    public void SetReviveTime(float reviveTime) => ReviveTime = reviveTime;
    public void SetReviveElapsedTime(float reviveElapsedTime) => ReviveElapsedTime = reviveElapsedTime;
    public void ResetReviveElapsedTime() => ReviveElapsedTime = 0;

}
