using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    public class SigilStorageManager : MonoBehaviour
    {
        public SigilStorageSO sigilStorageSO;

        private NormalSigilSO sigil_L;
        private NormalSigilSO sigil_R;
        private NormalSigilSO sigil_S;
        private NormalSigilSO sigil_F;
        private int index = 0;
        private List<NormalSigilSO> passiveSigils;


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
            passiveSigils = new List<NormalSigilSO>();
        }

        private void OnSigilChosen(SigilChosenEvent evt)
        {
            if (evt.normalSigilSO.isActiveSigil)
            {
                switch (evt.normalSigilSO.activeSigilType)
                {
                    case ActiveSigilType.L_Mouse:
                        sigil_L = evt.normalSigilSO;
                        sigilStorageSO.sigilTexture0 = sigil_L.sigilIcon;
                        break;
                    case ActiveSigilType.R_Mouse:
                        sigil_R = evt.normalSigilSO;
                        sigilStorageSO.sigilTexture1 = sigil_R.sigilIcon;
                        break;
                    case ActiveSigilType.Space:
                        sigil_S = evt.normalSigilSO;
                        sigilStorageSO.sigilTexture2 = sigil_S.sigilIcon;
                        break;
                    case ActiveSigilType.F:
                        sigil_F = evt.normalSigilSO;
                        sigilStorageSO.sigilTexture3 = sigil_F.sigilIcon;
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
                            sigilStorageSO.pSigil1 = evt.normalSigilSO.sigilIcon;
                            break;
                        case 1:
                            sigilStorageSO.pSigil2 = evt.normalSigilSO.sigilIcon;
                            break;
                        case 2:
                            sigilStorageSO.pSigil3 = evt.normalSigilSO.sigilIcon;
                            break;
                        case 3:
                            sigilStorageSO.pSigil4 = evt.normalSigilSO.sigilIcon;
                            break;
                        case 4:
                            sigilStorageSO.pSigil5 = evt.normalSigilSO.sigilIcon;
                            break;
                        case 5:
                            sigilStorageSO.pSigil6 = evt.normalSigilSO.sigilIcon;
                            break;
                        case 6:
                            sigilStorageSO.pSigil7 = evt.normalSigilSO.sigilIcon;
                            break;
                        case 7:
                            sigilStorageSO.pSigil8 = evt.normalSigilSO.sigilIcon;
                            break;
                        case 8:
                            sigilStorageSO.pSigil9 = evt.normalSigilSO.sigilIcon;
                            break;
                        case 9:
                            sigilStorageSO.pSigil10 = evt.normalSigilSO.sigilIcon;
                            break;
                        case 10:
                            sigilStorageSO.pSigil11 = evt.normalSigilSO.sigilIcon;
                            break;
                        case 11:
                            sigilStorageSO.pSigil12 = evt.normalSigilSO.sigilIcon;
                            break;
                    }
                    passiveSigils.Add(evt.normalSigilSO);
                    index++;
                }
            }
        }
    }
}