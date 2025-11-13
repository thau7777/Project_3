using UnityEngine;

namespace Turnbase
{
    [CreateAssetMenu(fileName = "New ContinuousVFX Settings", menuName = "Scriptable Objects/Flyweight Turnbase/ContinuousVFX Settings")]
    public class ContinuousVFXSettings2 : FlyweightSettings
    {
        public override Flyweight Create()
        {
            var go = Instantiate(prefab);
            go.name = prefab.name;
            var continuousVFX = go.GetOrAdd<ContinuousVFX>();

            continuousVFX.settings = this;

            return continuousVFX;
        }
    }
}