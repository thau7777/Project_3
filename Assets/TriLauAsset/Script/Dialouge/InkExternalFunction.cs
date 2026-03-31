using Ink.Runtime;
using MyRule.Event;
using MyRule.UI;
using System;
using UnityEngine;
namespace MyRule
{
    public class InkExternalFunction
    {
        public void Bind(Story story)
        {
            story.BindExternalFunction("OpenStore", () => OpenStore());
            story.BindExternalFunction("TriggerMiniGame", () => TriggerMiniGame());
            story.BindExternalFunction("ChosenSigil", (string sigilName) => ChosenSigil(sigilName));
            story.BindExternalFunction("UpdateRune", (int amount) => UpdateRune(amount));
            story.BindExternalFunction("TradeSigilByRune", (int rune, string sigilName) => TradeSigilByRune(rune, sigilName));
            story.BindExternalFunction("TradeSigilBySigil", (string fromSigil, string toSigil) => TradeSigilBySigil(fromSigil, toSigil));
            story.BindExternalFunction("BlockEarnRune", (int number) => LockEarnRune(number));
            story.BindExternalFunction("UpdateHealth", (int health) => UpdateHealth(health));
        }

        public void Unbind(Story story)
        {
            story.UnbindExternalFunction("OpenStore");
            story.UnbindExternalFunction("TriggerMiniGame");
            story.UnbindExternalFunction("ChosenSigil");
            story.UnbindExternalFunction("UpdateRune");
            story.UnbindExternalFunction("TradeSigilByRune");
            story.UnbindExternalFunction("TradeSigilBySigil");
            story.UnbindExternalFunction("BlockEarnRune");
            story.UnbindExternalFunction("UpdateHealth");
        }

        private void OpenStore()
        {
            EventBus<SwitchPanelEvent>.Raise(new SwitchPanelEvent(PanelType.Store));
        }

        private void TriggerMiniGame()
        {
            Debug.Log("TriggerMiniGame");
            EventBus<TriggerMiniGameEvent>.Raise(new TriggerMiniGameEvent());
        }

        private void ChosenSigil(string sigilName)
        {
            Debug.Log($"ChosenSigil: {sigilName}");
            SigilSO sigilSO = SigilCollectionManager.Instance.GetSigilSOByName(sigilName);
            SigilStorageManager.Instance.AddSigilToStorage(sigilSO);
        }

        private void UpdateRune(int amount)
        {
            EventBus<ReceiveRuneEvent>.Raise(new ReceiveRuneEvent(amount));
        }

        private void TradeSigilByRune(int rune, string sigilName)
        {
            EventBus<SpendRuneEvent>.Raise(new SpendRuneEvent(rune));
            SigilSO sigilSO = SigilCollectionManager.Instance.GetSigilSOByName(sigilName);
            SigilStorageManager.Instance.AddSigilToStorage(sigilSO);
        }

        private void TradeSigilBySigil(string fromSigil, string toSigil)
        {
            SigilSO fromSigilSO = SigilCollectionManager.Instance.GetSigilSOByName(fromSigil);
            SigilSO toSigilSO = SigilCollectionManager.Instance.GetSigilSOByName(toSigil);

            int index = 0;

            if (fromSigilSO.sigilType == SigilType.Active && toSigilSO.sigilType == SigilType.Active)
            {
                SigilStorageManager.Instance.SigilStorageData.GetIndexOfActiveSigil(fromSigil);
            }
            else if (fromSigilSO.sigilType == SigilType.Passive && toSigilSO.sigilType == SigilType.Passive)
            {
                SigilStorageManager.Instance.SigilStorageData.GetIndexOfPassiveSigil(fromSigil);
            }
            else if (fromSigilSO.sigilType == SigilType.Active && toSigilSO.sigilType == SigilType.Passive)
            {
                SigilStorageManager.Instance.SigilStorageData.GetIndexOfActiveSigil(fromSigil);
                SigilStorageManager.Instance.SigilStorageData.RemoveActiveSigil(index);
                            }
            else if (fromSigilSO.sigilType == SigilType.Passive && toSigilSO.sigilType == SigilType.Active)
            {
                SigilStorageManager.Instance.SigilStorageData.GetIndexOfPassiveSigil(fromSigil);
                SigilStorageManager.Instance.SigilStorageData.RemovePassiveSigil(index);
            }

            SigilStorageManager.Instance.AddSigilToStorage(toSigilSO, index);
        }

        private void LockEarnRune(int turn)
        {
            RuneManger.Instance.SetLockReceiveTurn(turn);
        }

        private void UpdateHealth(int health)
        {
            CharacterManager.Instance.IncreaseHealth(health);
        }
    }
}