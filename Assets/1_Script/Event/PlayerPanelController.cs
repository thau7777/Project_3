using UnityEngine;

public class PlayerPanelController : MonoBehaviour
{
    public string panelID;

    private Animator animator;

    private EventBinding<HidePanelEvent> hidePanelBinding;
    private EventBinding<ShowPanelEvent> showPanelBinding;
    private EventBinding<OnUIAction> showActionConfirmBinding;
    private EventBinding<OffUIAction> hideActionConfirmBinding;



    private void Awake()
    {
        hidePanelBinding = new EventBinding<HidePanelEvent>(HidePanel);
        showPanelBinding = new EventBinding<ShowPanelEvent>(ShowPanel);
        showActionConfirmBinding = new EventBinding<OnUIAction>(ShowActionConfirm);
        hideActionConfirmBinding = new EventBinding<OffUIAction>(HideActionConfirm);

        EventBus<HidePanelEvent>.Register(hidePanelBinding);
        EventBus<ShowPanelEvent>.Register(showPanelBinding);
        EventBus<OnUIAction>.Register(showActionConfirmBinding);
        EventBus<OffUIAction>.Register(hideActionConfirmBinding);
        Debug.Log($"Panel '{panelID}': Đăng ký Event Bus trong Awake().");
    }


    private void OnDestroy()
    {
        EventBus<HidePanelEvent>.Deregister(hidePanelBinding);
        EventBus<ShowPanelEvent>.Deregister(showPanelBinding);
        EventBus<OnUIAction>.Deregister(showActionConfirmBinding);
        EventBus<OffUIAction>.Deregister(hideActionConfirmBinding);
    }

    private void HidePanel(HidePanelEvent e)
    {
        if (e.panelName.Equals(panelID, System.StringComparison.OrdinalIgnoreCase))
        {
            gameObject.SetActive(false);
            Debug.Log($"Panel '{panelID}' ẩn thành công.");
        }
    }

    private void ShowPanel(ShowPanelEvent e)
    {
        if (e.panelName.Equals(panelID, System.StringComparison.OrdinalIgnoreCase))
        {
            gameObject.SetActive(true);
            Debug.Log($"Panel '{panelID}' hiện thành công.");
        }
    }

    private void HideActionConfirm(OffUIAction e)
    {
        if (e.panelName.Equals(panelID, System.StringComparison.OrdinalIgnoreCase))
        {
            animator = GetComponent<Animator>();
            animator.Play("OffUI");
            Debug.Log($"Panel Action '{panelID}' hiện thành công.");
        }
    }
    
    private void ShowActionConfirm(OnUIAction e)
    {
        if (e.panelName.Equals(panelID, System.StringComparison.OrdinalIgnoreCase))
        {
            animator = GetComponent<Animator>();
            animator.Play("OnUI");
            Debug.Log($"Panel Action '{panelID}' hiện thành công.");
        }
    }

}
