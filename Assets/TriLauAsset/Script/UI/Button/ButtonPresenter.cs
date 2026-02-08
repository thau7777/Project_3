using MyRule.Audio;
using UnityEngine;

namespace MyRule.UI
{
    public class ButtonPresenter
    {
        private IButtonView view;
        
        public ButtonPresenter(IButtonView buttonView)
        { 
            this.view = buttonView;
        }

        public void CleanUp()
        {
            this.view = null;
        }
    }
}