using MyRule.Event;
using System.Collections.Generic;
using UnityEngine;

namespace MyRule.UI
{
    public class SigilCollectionView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private GameObject noCollection;
        [SerializeField] private Transform sigilsParent;
        [SerializeField] private GameObject sigilViewPreb;

        private List<GameObject> sigilViews = new List<GameObject>();

        private EventBinding<UpdateSigilCollectionEvent> updateSigilCollectionEvent;

        private void OnEnable()
        {
            updateSigilCollectionEvent = new EventBinding<UpdateSigilCollectionEvent>(UpdateSigilCollection);
            EventBus<UpdateSigilCollectionEvent>.Register(updateSigilCollectionEvent);
        }

        private void OnDisable()
        {
            EventBus<UpdateSigilCollectionEvent>.Deregister(updateSigilCollectionEvent);
        }

        private void UpdateSigilCollection(UpdateSigilCollectionEvent evt)
        {
            ResetSigilsView();

            _canvasGroup.alpha = 1;
            noCollection.SetActive(false);

            foreach (var sigil in evt.data.ActiveSigils)
            {
                GameObject sigilViewObj = Instantiate(sigilViewPreb, sigilsParent);
                LobbySigilView sigilView = sigilViewObj.GetComponent<LobbySigilView>();
                sigilView.SetSigil(sigil.Value);
                sigilViews.Add(sigilViewObj);
            }

            foreach (var sigil in evt.data.PassiveSigils)
            {
                GameObject sigilViewObj = Instantiate(sigilViewPreb, sigilsParent);
                LobbySigilView sigilView = sigilViewObj.GetComponent<LobbySigilView>();
                sigilView.SetSigil(sigil.Value);
                sigilViews.Add(sigilViewObj);
            }
        }

        private void ResetSigilsView()
        {
            foreach (var sigil in sigilViews)
            {
                Destroy(sigil);
            }

            sigilViews.Clear();
        }
    }
}