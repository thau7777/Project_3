using UnityEngine;

namespace MyRule
{
    public enum PlanetType
    {
        GreenLand,
        Desert,
        IceLand,
    }

    [CreateAssetMenu(fileName = "PlanetSO", menuName = "Scriptable Objects/PlanetSO")]
    public class PlanetSO : ScriptableObject
    {
        public PlanetType planetType;
    }
}
