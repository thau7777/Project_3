using UnityEngine;

namespace Turnbase
{
    public class OpenPanelUI : MonoBehaviour
    {
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void OnEnable()
        {
            animator.Play("OpenPanelUI");
        }
    }

}