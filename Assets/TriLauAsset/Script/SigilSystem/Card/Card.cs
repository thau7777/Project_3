using UnityEngine;

namespace MyRule
{
    public class Card : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private bool showing = false;

        public bool Showing
        {
            get { return showing; }
            set 
            { 
                showing = value;
                
                animator.SetBool("Show", showing);
            }
        }

        private void Start()
        {
            animator = GetComponent<Animator>();
        }
    }
}