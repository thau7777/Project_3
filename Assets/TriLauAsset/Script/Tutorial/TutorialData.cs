using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MyRule
{
    [Serializable]
    public class TutorialData
    {
        [JsonProperty] private Dictionary<string, bool> _tutorials;

        [JsonIgnore] public Dictionary<string, bool> Tutorials => _tutorials;

        public TutorialData() 
        {
            _tutorials = new Dictionary<string, bool>();
        }

        public bool HasCompletedAllTutorial()
        {
            foreach (var value in _tutorials.Values)
            {
                if (!value) return false;
            }
            return true;
        }
    }
}


