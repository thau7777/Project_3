using Cysharp.Threading.Tasks;
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

        private List<GameObject> planetEneymiesViewObj = new List<GameObject>();

        public async void Show(PlanetSO planetSO)
        {
            await UniTask.Delay(200);

            _group.DOFade(1f, fadeDuration);
            planetName.text = planetSO.planetName;
            planetImage.sprite = planetSO.image;
            planetDescription.text = planetSO.planetDescription;

            for (int i = 0; i < planetSO.mapEnemies.enemies.Count; i++)
            {
                GameObject ennemy = Instantiate(planetEnemyView, planetEnemiesParent);
                PlanetEnemyView enemyView = ennemy.GetComponent<PlanetEnemyView>();
                enemyView.SetEnemyData(planetSO.mapEnemies.enemies[i]);
                planetEneymiesViewObj.Add(ennemy.gameObject);
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