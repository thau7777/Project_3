using UnityEngine;

public class DamageEvent 
{
    public Vector3 position;
    public int amount;
    public Color color;
    public bool isCrit;

    public DamageEvent(Vector3 pos, int amt, Color col, bool crit)
    {
        position = pos;
        amount = amt;
        color = col;
        isCrit = crit;
    }
}