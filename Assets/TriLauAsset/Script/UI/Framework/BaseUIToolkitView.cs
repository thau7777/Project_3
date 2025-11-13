using UnityEngine;
using UnityEngine.UIElements;

namespace MyRule.UI
{
    public abstract class BaseUIToolkitView : BaseUIView
    {
        private UIDocument document;
        protected VisualElement root;

        protected override void OnEnable()
        {
            base.OnEnable();

            document = GetComponent<UIDocument>();
            root = document.rootVisualElement;
        }
    }
}