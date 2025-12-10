using UnityEngine;

[CreateAssetMenu(fileName = "New Minion Data", menuName = "Scriptable Objects/TopDownMinionData")]
public class MinionData : ScriptableObject
{
    [field: SerializeField] public MinionsManager.MinionKind Kind { get; set; }
    [field: SerializeField] public GameObject MinionPrefab { get; set; }
    [field: SerializeField, ReadOnly] public int MaxHealth { get; private set; } = 100;
    [field: SerializeField, ReadOnly] public int MaxMana { get; private set; } = 100;
    [field: SerializeField, ReadOnly] public int BaseAttackDamage { get; private set; } = 20;

    public void InitializeMinionStats()
    {
        MaxHealth = 100;
        MaxMana = 100;
        BaseAttackDamage = 20;
    }

}
