using Turnbase;

public class StatusEffectChangedEvent
{
    public Character TargetCharacter { get; }

    public StatusEffectChangedEvent(Character target)
    {
        TargetCharacter = target;
    }
}