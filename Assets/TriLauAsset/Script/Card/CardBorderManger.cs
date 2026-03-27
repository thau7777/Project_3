using UnityEngine;

namespace MyRule
{
    public class CardBorderManger : Singleton<CardBorderManger>
    {
        [SerializeField] private Material[] cardBorderMats;

        public Material GetMaterial(int i) => cardBorderMats[i];
    }
}