using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace MyRule
{
    [Serializable]
    public class CardPoolConfig
    {
        public Card prefab;
        public int initialSize = 1;
    }

    public class CardPoolManager : Singleton<CardPoolManager>
    {
        [SerializeField] private List<CardPoolConfig> configs;

        [SerializeField] private bool collectionCheck = true;
        [SerializeField] private int maxSize = 10;

        private Dictionary<string, IObjectPool<Card>> pools = new();

        protected override void Awake()
        {
            base.Awake();

            Init();
        }

        private void Init()
        {
            foreach (var config in configs)
            {
                var pool = CreatePool(config);
                if (config.prefab.SigilSO != null) pools[config.prefab.SigilSO.id] = pool;
                else pools["none"] = pool;
            }
        }

        private IObjectPool<Card> CreatePool(CardPoolConfig config)
        {
            return new ObjectPool<Card>(
                () => CreateCard(config.prefab),
                OnGetFromPool,
                OnReleaseToPool,
                OnDestroyCardObject,
                collectionCheck,
                config.initialSize,
                maxSize
            );
        }

        private Card CreateCard(Card prefab)
        {
            Card card = Instantiate(prefab);
            card.transform.SetParent(transform);
            card.transform.position = transform.position;
            card.SetPool(null);
            return card;
        }

        private async void OnGetFromPool(Card card)
        {
            card.transform.DOKill(true);
            card.transform.localScale = Vector3.one;
            card.transform.localRotation = Quaternion.Euler(0, 180, 0);

            card.gameObject.SetActive(true);
            await card.OnSpawn();
        }

        private async void OnReleaseToPool(Card card)
        {
            await card.OnDespawn();
            card.transform.SetParent(transform);
            card.transform.position = transform.position;
            card.gameObject.SetActive(false); 
        }

        private void OnDestroyCardObject(Card card)
        {
            Destroy(card.gameObject);
        }

        public Card Spawn(string id)
        {
            var pool = pools.ContainsKey(id) ? pools[id] : pools["none"];
            var card = pool.Get();
            card.SetPool(pool);
            return card;
        }
    }
}