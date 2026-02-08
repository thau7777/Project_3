using UnityEngine;
using UnityEngine.EventSystems;

public class MoleUI : MonoBehaviour, IPointerClickHandler
{
    public float lifeTime = 2f;
    private UI_MoleSpawner _spawner;
    private int _myIndex = -1;

    public void SetMySlot(UI_MoleSpawner spawner, int index)
    {
        _spawner = spawner;
        _myIndex = index;
    }

    void OnEnable()
    {
        CancelInvoke(nameof(HideMole));
        Invoke(nameof(HideMole), lifeTime);

        // Nếu có Animator, hãy Trigger hiệu ứng "Up" ở đây
        // GetComponent<Animator>().SetTrigger("Up");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        HideMole();
    }

    void HideMole()
    {
        if (_spawner != null && _myIndex != -1)
        {
            _spawner.ReleaseSlot(_myIndex);
        }
        gameObject.SetActive(false);
    }
}