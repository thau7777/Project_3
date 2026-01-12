using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    public class SigilStorageManager : MonoBehaviour
    {
        public SigilStorageSO sigilStorageSO;

        private SigilSO sigil_L;
        private SigilSO sigil_R;
        private SigilSO sigil_S;
        private SigilSO sigil_F;
        private int index = 0;
        private List<SigilSO> passiveSigils;


        private EventBinding<SigilChosenEvent> sigilChosenEventBinding;

        private void OnEnable()
        {
            sigilChosenEventBinding = new EventBinding<SigilChosenEvent>(OnSigilChosen);
            EventBus<SigilChosenEvent>.Register(sigilChosenEventBinding);
        }

        private void OnDisable()
        {
            EventBus<SigilChosenEvent>.Deregister(sigilChosenEventBinding);
        }

        private void Awake()
        {
            passiveSigils = new List<SigilSO>();
        }

        private void OnSigilChosen(SigilChosenEvent evt)
        {
            if (evt.normalSigilSO.isActiveSigil)
            {
                switch (evt.normalSigilSO.activeSigilType)
                {
                    case ActiveSigilType.L_Mouse:
                        sigil_L = evt.normalSigilSO;
                        sigilStorageSO.sigil_L = sigil_L;
                        break;
                    case ActiveSigilType.R_Mouse:
                        sigil_R = evt.normalSigilSO;
                        sigilStorageSO.sigil_R = sigil_R;
                        break;
                    case ActiveSigilType.Space:
                        sigil_S = evt.normalSigilSO;
                        sigilStorageSO.sigil_S = sigil_S;
                        break;
                    case ActiveSigilType.F:
                        sigil_F = evt.normalSigilSO;
                        sigilStorageSO.sigil_F = sigil_F;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (index < 12)
                {
                    switch (index)
                    {
                        case 0:
                            sigilStorageSO.pSigil1 = evt.normalSigilSO;
                            break;
                        case 1:
                            sigilStorageSO.pSigil2 = evt.normalSigilSO;
                            break;
                        case 2:
                            sigilStorageSO.pSigil3 = evt.normalSigilSO;
                            break;
                        case 3:
                            sigilStorageSO.pSigil4 = evt.normalSigilSO;
                            break;
                        case 4:
                            sigilStorageSO.pSigil5 = evt.normalSigilSO;
                            break;
                        case 5:
                            sigilStorageSO.pSigil6 = evt.normalSigilSO;
                            break;
                        case 6:
                            sigilStorageSO.pSigil7 = evt.normalSigilSO;
                            break;
                        case 7:
                            sigilStorageSO.pSigil8 = evt.normalSigilSO;
                            break;
                    }
                    passiveSigils.Add(evt.normalSigilSO);
                    index++;
                }
            }
        }
    }
}