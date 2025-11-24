using UnityEngine;

namespace Turnbase
{
    [CreateAssetMenu(fileName = "New ImpactVFXSetting Settings", menuName = "Scriptable Objects/Flyweight Turnbase/ImpactVFXSetting Settings")]
    public class ImpactVFXSetting_TB : FlyweightSettings_TB
    {
        public override Flyweight_TB Create()
        {
            var go = Instantiate(prefab);
            go.name = prefab.name;
            var impactVFX = go.GetOrAdd<ImpactVFX_TB>();

            impactVFX.settings = this;

            return impactVFX;
        }
    }
}