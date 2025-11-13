using UnityEngine;

namespace Turnbase
{
    [CreateAssetMenu(fileName = "New ContinuousVFX Settings", menuName = "Scriptable Objects/Flyweight Turnbase/ContinuousVFX Settings")]
    public class ContinuousVFXSettings2 : FlyweightSettings2
    {
        public override Flyweight2 Create()
        {
            var go = Instantiate(prefab);
            go.name = prefab.name;
            var continuousVFX = go.GetOrAdd<ContinuousVFX2>();

            continuousVFX.settings = this;

            return continuousVFX;
        }
    }
}