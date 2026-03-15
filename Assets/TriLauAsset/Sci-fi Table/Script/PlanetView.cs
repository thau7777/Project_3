using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyRule.UI
{
    public class PlanetView : Singleton<PlanetView>
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private float fadeDuration = 0.2f;
        [SerializeField] private TextMeshProUGUI planetName;
        [SerializeField] private Image planetImage;
        [SerializeField] private TextMeshProUGUI planetDescription;
        [SerializeField] private Transform planetEnemiesParent;
        [SerializeField] private GameObject planetEnemyView;

        private List<GameObject> planetEneymiesViewObj;

        public void Show(PlanetSO planetSO)
        {
            _group.DOFade(1f, fadeDuration);
            planetName.text = planetSO.planetName;
            planetImage.sprite = planetSO.image;
            planetDescription.text = planetSO.planetDescription;

            for (int i = 0; i < planetSO.mapEnemies.enemies.Count; i++)
            {
                var ennemyView = Instantiate(planetEnemyView, planetEnemiesParent).GetComponent<PlanetEnemyView>();
                ennemyView.SetEnemyData(planetSO.mapEnemies.enemies[i]);
                planetEneymiesViewObj.Add(ennemyView.gameObject);
            }
        }

        public void Hide()
        {
            _group.DOFade(0f, fadeDuration);

            foreach (var planetView in planetEneymiesViewObj)
            {
                Destroy(planetView.gameObject);
            }

            planetEneymiesViewObj.Clear();
        }
    }
}