using UnityEngine;
using UnityEngine.Playables;

namespace MyRule
{
    public class TimelineController : MonoBehaviour
    {
        [SerializeField] private PlayableDirector playableDirector;
        [SerializeField] private DataSO dataSO;

        private void Start()
        {
            if (dataSO.isFrist) playableDirector.Play();
        }
    }
}