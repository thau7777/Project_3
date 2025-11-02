using UnityEngine;

public abstract class PopupMiniGameUIBase : MonoBehaviour
{
    protected InputReader inputReader;
    protected PopupGameManager manager;

    public virtual void Init(InputReader inputReader, PopupGameManager manager)
    {
        this.inputReader = inputReader;
        this.manager = manager;
    }

    public virtual void Show() => gameObject.SetActive(true);
    public virtual void Hide() => gameObject.SetActive(false);
}
