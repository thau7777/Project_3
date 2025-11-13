namespace MyRule.UI
{
    public interface IButtonView
    {
        ButtonType Type { get; }
        void Select();
        void Deselect();
        void Submit();
    }
}