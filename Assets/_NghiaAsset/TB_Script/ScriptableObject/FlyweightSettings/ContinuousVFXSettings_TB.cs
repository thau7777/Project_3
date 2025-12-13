using UnityEngine;

namespace Turnbase
{
    [CreateAssetMenu(fileName = "New ContinuousVFX Settings", menuName = "Scriptable Objects/Flyweight Turnbase/ContinuousVFX Settings")]
    public class ContinuousVFXSettings_TB : FlyweightSettings_TB
    {
        public override Flyweight_TB Create()
        {
            var go = Instantiate(prefab);
            go.name = prefab.name;
            var continuousVFX = go.GetOrAdd<ContinuousVFX_TB>();

            continuousVFX.settings = this;

            return continuousVFX;
        }
    }
}