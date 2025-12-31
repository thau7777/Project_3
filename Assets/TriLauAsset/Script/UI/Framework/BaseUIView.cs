using UnityEngine;

namespace MyRule.UI
{
    public enum PanelType
    {
        MainMenu,
        Settings,
        SaveFiles,
        Credits,
        PauseMenu,
        Inventory,
        CharacterStats,
        None
    }

    public abstract class BaseUIView : MonoBehaviour, IBaseUIView
    {
        protected PanelType panelType;
        public PanelType Type => panelType;

        private BaseUIPresenter presenter;

        protected virtual void OnEnable()
        {
            presenter = new BaseUIPresenter(this);
        }

        protected virtual void OnDisable()
        {
            presenter?.Cleanup();
        }
        
        protected virtual void Awake() { }

        protected virtual void Start() { }

        public abstract void Show();
        public abstract void Hide();
    }
}