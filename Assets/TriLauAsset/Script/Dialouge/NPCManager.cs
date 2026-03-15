using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace MyRule
{
    [Serializable]
    public class NPCInMatch
    {
        public GameObject camObj;
        public DialougeTrigger DialougeTrigger;
    }

    public class NPCManager : Singleton<NPCManager>
    {
        [SerializeField] private NPCInMatch[] nPCInMatches;
        [SerializeField] private NPCInMatch salesman;
        [SerializeField] private NPCInMatch tae;

        private NPCInMatch currentNPC;

        public void RandomNPC()
        {
            int index = UnityEngine.Random.Range(0, nPCInMatches.Length);
            TriggerNPC(nPCInMatches[index]);
        }

        public void TriggetStore() => TriggerNPC(salesman);

        public async UniTask TriggerTAE()
        {
            await UniTask.WaitUntil(() => GameSystemManager.Instance != null);

            if (MatchManager.Instance.MatchData.IsNewMatch)
            {
                TriggerNPC(tae);
            }
        }

        private async void TriggerNPC(NPCInMatch npc)
        {
            currentNPC = npc;

            npc.camObj.SetActive(true);

            await UniTask.Delay(800);

            npc.DialougeTrigger.Trigger();
        }

        public void ExitDialogue()
        {
            currentNPC.camObj.SetActive(false);
        }
    }
}