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
        TabView,
        Profile,
        Diary,
        LobbyShop,
        Portal,
        Stats,
        HUD,
        Store,
        None
    }

    public abstract class BaseUIView : MonoBehaviour, IBaseUIView
    {
        [SerializeField] protected InputReader inputReader;
        [SerializeField] protected PanelType panelType;
        public PanelType Type => panelType;

        protected bool isActive;

        public bool IsActive
        {
            get => isActive;
            set => isActive = value;
        }

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