using Newtonsoft.Json;
using System;

namespace MyRule
{
    [Serializable]
    public class SigilStorageSlotData
    {
        private int _index;
        private SigilData _data;

        public int Index => _index;
        public SigilData Data => _data;

        public SigilStorageSlotData(int index, SigilData data)
        {
            _index = index;
            _data = data;
        }
    }

    [Serializable]
    public class SigilStorageData
    {
        [JsonProperty] private SigilData[] _activeSigils;
        [JsonProperty] protected SigilData[] _passiveSigils;

        [JsonIgnore] public SigilData[] ActiveSigils => _activeSigils;
        [JsonIgnore] public SigilData[] PassiveSigils => _passiveSigils;

        public SigilStorageData()
        {
            _activeSigils = new SigilData[12];
            _passiveSigils = new SigilData[20];
        }

        // Add
        public bool TryAdd(SigilData[] row, int index, SigilData data)
        {
            if (index < 0 || index >= row.Length) return false;
            row[index] = data;
            return true;
        }

        public bool TryAddActiveSigil(int index, SigilData data) => TryAdd(_activeSigils, index, data);
        public bool TryAddPassiveSigil(int index, SigilData data) => TryAdd(_passiveSigils, index, data);

        // Remove
        public void RemoveActiveSigil(int index) => _activeSigils[index] = null;
        public void RemovePassiveSigil(int index) => _passiveSigils[index] = null;

        // Swap 
        private void Swap(SigilData[] rowA, int indexA, SigilData[] rowB, int indexB)
        {
            (rowA[indexA], rowB[indexB]) = (rowB[indexB], rowA[indexA]);
        }

        public void SwapActiveSigil(int indexA, int indexB)
            => Swap(_activeSigils, indexA, _activeSigils, indexB);
        
        // Check full
        private bool IsFull(SigilData[] row)
            => Array.TrueForAll(row, slot => slot != null);

        public bool IsActiveSigilFull() => IsFull(_activeSigils);
        public bool IsPassiveSigilFull() => IsFull(_passiveSigils);

        // Get Empty
        private int GetFirstEmptySlot(SigilData[] row)
            => Array.IndexOf(row, null);

        public int GetFirstEmptySlotActive() => GetFirstEmptySlot(_activeSigils);
        public int GetFirstEmptySlotPassive() => GetFirstEmptySlot(_passiveSigils);

        // Get index
        public int GetIndexOfSigil(string sigilName, SigilData[] sigilDatas)
        {
            for (int i = 0; i < sigilDatas.Length; i++)
            {
                if (sigilDatas[i].Name == sigilName) return i;
            }

            return -1;
        }

        public int GetIndexOfActiveSigil(string name) => GetIndexOfSigil(name, _activeSigils);

        public int GetIndexOfPassiveSigil(string name) => GetIndexOfSigil(name, _passiveSigils);
    }
}