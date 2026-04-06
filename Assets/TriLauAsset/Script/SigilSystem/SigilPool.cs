using MyRule.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MyRule
{
    [JsonObject]
    public class SigilPool
    {
        [JsonProperty] private List<SigilData> _activeSigilPool;
        [JsonProperty] private List<SigilData> _passiveSigilPool;

        [JsonConstructor]
        public SigilPool() 
        {
            _activeSigilPool = new List<SigilData>();
            _passiveSigilPool = new List<SigilData>();
        }

        public void CreatePool(List<SigilData> sigils)
        {
            _activeSigilPool.Clear();
            _passiveSigilPool.Clear();

            foreach (SigilData sigil in sigils)
            {
                if (sigil.SigilType == SigilType.Active)
                {
                    _activeSigilPool.Add(sigil);
                }
                else
                {
                    _passiveSigilPool.Add(sigil);
                }
            }
        }

        public List<SigilData> GetActiveSigils(int count)
        {
            return RandomISigilPicker.GetRandomWeightedUnique(_activeSigilPool, count);
        }

        public List<SigilData> GetPassiveSigils(int count)
        {
            return RandomISigilPicker.GetRandomWeightedUnique(_passiveSigilPool, count);
        }

        public List<SigilData> GetMixedSigils(int totalCount, int minActive = 1)
        {
            List<SigilData> result = new List<SigilData>();

            var active = RandomISigilPicker.GetRandomWeightedUnique(_activeSigilPool, minActive);
            result.AddRange(active);

            List<SigilData> remainingPool = new List<SigilData>();
            remainingPool.AddRange(_activeSigilPool);
            remainingPool.AddRange(_passiveSigilPool);

            foreach (var item in result)
            {
                remainingPool.Remove(item);
            }

            int remainCount = totalCount - result.Count;

            var rest = RandomISigilPicker.GetRandomWeightedUnique(remainingPool, remainCount);
            result.AddRange(rest);

            return result;
        }

        public void RemoveSigil(SigilData sigil)
        {
            if (sigil.SigilType == SigilType.Active)
            {
                _activeSigilPool.Remove(sigil);
            }
            else
            {
                _passiveSigilPool.Remove(sigil);
            }
        }

        public SigilData GetActiveSigilById(string id)
        {
            SigilData sigil = _activeSigilPool.Find(x => x.Id == id);
            if (sigil == null) return null; 
            _activeSigilPool.Remove(sigil);
            return sigil;
        }

        public SigilData GetPassiveSigilById(string id)
        {
            SigilData sigil = _passiveSigilPool.Find(x => x.Id == id);
            if (sigil == null) return null;
            _passiveSigilPool.Remove(sigil);
            return sigil;
        }
    }
}