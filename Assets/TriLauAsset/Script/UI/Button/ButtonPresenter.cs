using UnityEngine;

namespace MyRule.UI
{
    public class ButtonPresenter
    {
        IButtonView view;

        public ButtonPresenter(IButtonView buttonView)
        { 
            view = buttonView;
        }

        public void CleanUp()
        {
            view = null;
        }
    }
}