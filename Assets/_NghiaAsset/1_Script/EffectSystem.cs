using UnityEngine;

namespace Turnbase
{
    public class EffectSystem : Singleton<EffectSystem>
    {
        [Header("Global Config")]
        public int defaultBreakDuration;

        [Header("Break Effect Config")]
        public FlyweightSettings_TB breakVFXSettings;
        public Sprite breakIcon;

        [Header("Stun Effect Config")]
        public FlyweightSettings_TB stunVFXSettings;
        public Sprite stunIcon;

        public void TriggerBreak(Character target, int duration)
        {
            if (target == null || target.debuffManager == null) return;

            target.debuffManager.ApplyBreakDebuff(defaultBreakDuration, breakVFXSettings, breakIcon);

            Debug.Log($"<color=yellow>[EffectSystem]</color> Triggered BREAK on {target.name}");
        }

        public void TriggerStun(Character target, int duration)
        {
            if (target == null || target.debuffManager == null) return;

            target.debuffManager.ApplyStunDebuff(defaultBreakDuration, stunVFXSettings, stunIcon);
        }
    }
}