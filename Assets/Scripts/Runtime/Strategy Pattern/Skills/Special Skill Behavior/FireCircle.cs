using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "Fire Circle",
    menuName = "Scriptable Objects/StrategyPattern/Special Skills/Fire Circle"
)]
public class FireCircle : SkillStrategy
{
    [MinMaxSlider(0, 1)]
    public Vector2 hitboxOnOffTime = new Vector2(0, 0.1f);
    public EffectData burnEffect;

    private SkillStrategyContext _skillContext;
    public override void Execute(IStrategyContext context)
    {
        _skillContext = context as SkillStrategyContext;
        if (_skillContext == null) return;

        SpawnController();
    }
    private void SpawnController()
    {
        ContinousVFX controller = FlyweightFactory.Spawn(_mainSkillVfxSettings) as ContinousVFX;
        if (controller != null)
        {
            ContinousVFXSettings skillVfxSettings = _mainSkillVfxSettings as ContinousVFXSettings;
            controller.FlyweightInitialize(_skillContext.spawnTransform.AddLocal(positionOffset.x, positionOffset.y, positionOffset.z), parent: _skillContext.origin);
            controller.InitializeVFX(skillVfxSettings.DefaultSize);

            GameObject skillVfx = controller.transform.GetChild(0).gameObject;

            HitBoxHandler hitBoxHandler = skillVfx.GetOrAdd<HitBoxHandler>();
            hitBoxHandler.DodgeLayers = DodgeLayers;
            hitBoxHandler.HitboxOnOffTime = hitboxOnOffTime;

            var damageDealer = skillVfx.GetOrAdd<DamageDealer>();
            damageDealer.Damage = Damage;
            damageDealer.KnockbackForce = 3;

            var effectApplier = skillVfx.GetOrAdd<EffectApplier>();
            effectApplier.SetEffects(new List<EffectData> { burnEffect });


            controller.gameObject.AddComponent<FireCircleController>();

        }
    }

}
