using Cysharp.Threading.Tasks;
using Ink.Runtime;
using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    public class InkVariable : IGameData
    {
        private Dictionary<string, Ink.Runtime.Object> variables = new Dictionary<string, Ink.Runtime.Object>();

        private Story story;

        public InkVariable(Story story)
        {
            this.story = story;
        }

        public void SyncVariablesAndStartListening(Story story)
        {
            SyncVariablesToStory(story);
            story.variablesState.variableChangedEvent += UpdateVariableState;
        }

        public void StopListening(Story story)
        {
            story.variablesState.variableChangedEvent -= UpdateVariableState;
        }

        public void UpdateVariableState(string name, Ink.Runtime.Object value)
        {
            if (!variables.ContainsKey(name))
            {
                return;
            }
            variables[name] = value;
        }

        private void SyncVariablesToStory(Story story)
        {
            foreach (KeyValuePair<string, Ink.Runtime.Object> variable in variables)
            {
                story.variablesState.SetGlobal(variable.Key, variable.Value);
            }
        }

        public UniTask LoadData(GameData data)
        {
            variables.Clear();
            foreach (string name in story.variablesState)
            {
                Ink.Runtime.Object value = story.variablesState.GetVariableWithName(name);
                variables[name] = value;
            }

            foreach (var pair in data.DialougeData.KeyValuePairs)
            {
                if (!variables.ContainsKey(pair.Key)) continue;

                Ink.Runtime.Object inkValue = pair.Value switch
                {
                    bool b => new BoolValue(b),
                    long i => new IntValue((int)i),
                    int i => new IntValue(i),
                    string s => new StringValue(s),
                    _ => null
                };

                if (inkValue != null)
                    variables[pair.Key] = inkValue;
            }

            return UniTask.CompletedTask;
        }

        public void SaveData(GameData data)
        {
            foreach (var variable in variables)
            {
                object value = variable.Value switch
                {
                    BoolValue b => b.value,
                    IntValue i => i.value,
                    StringValue s => s.value,
                    _ => variable.Value.ToString()
                };
                data.DialougeData.KeyValuePairs[variable.Key] = value;
            }
        }

        public UniTask NewGame()
        {
            variables = new Dictionary<string, Ink.Runtime.Object>();

            return UniTask.CompletedTask;
        }
    }
}