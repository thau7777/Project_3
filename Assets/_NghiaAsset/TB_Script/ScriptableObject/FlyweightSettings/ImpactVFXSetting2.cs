using UnityEngine;

namespace Tunrbase
{
    [CreateAssetMenu(fileName = "New ImpactVFXSetting Settings", menuName = "Scriptable Objects/Flyweight Turnbase/ImpactVFXSetting Settings")]
    public class ImpactVFXSetting2 : FlyweightSettings
    {
        public override Flyweight Create()
        {
            var go = Instantiate(prefab);
            go.name = prefab.name;
            var impactVFX = go.GetOrAdd<ImpactVFX>();

            impactVFX.settings = this;

            return impactVFX;
        }
    }
}