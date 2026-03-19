using UnityEngine;

namespace MyRule
{
    public enum EMap
    {
        GreenLand,
        Desert,
        IceLand,
    }

    [CreateAssetMenu(fileName = "PlanetSO", menuName = "Scriptable Objects/PlanetSO")]
    public class PlanetSO : ScriptableObject
    {
        public EMap planetType;
        public Loader.EScene scene;
        public string planetName;
        public Sprite image;
        [TextArea(3, 4)]
        public string planetDescription;
        public MapEnemies mapEnemies;
    }
}
