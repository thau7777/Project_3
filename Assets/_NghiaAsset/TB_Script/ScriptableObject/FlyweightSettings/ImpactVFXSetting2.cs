using UnityEngine;

namespace Turnbase
{
    [CreateAssetMenu(fileName = "New ImpactVFXSetting Settings", menuName = "Scriptable Objects/Flyweight Turnbase/ImpactVFXSetting Settings")]
    public class ImpactVFXSetting2 : FlyweightSettings2
    {
        public override Flyweight2 Create()
        {
            var go = Instantiate(prefab);
            go.name = prefab.name;
            var impactVFX = go.GetOrAdd<ImpactVFX2>();

            impactVFX.settings = this;

            return impactVFX;
        }
    }
}