using UnityEngine;
using UnityEngine.EventSystems;

public class MoleUI : MonoBehaviour, IPointerClickHandler
{
    public float minLifeTime = 2f;
    public float maxLifeTime = 3f;

    private UI_MoleSpawner _spawner;
    private int _slotIndex;

    public void Init(UI_MoleSpawner spawner, int index)
    {
        _spawner = spawner;
        _slotIndex = index;
    }

    void OnEnable()
    {
        CancelInvoke();
        float lifeTime = Random.Range(minLifeTime, maxLifeTime);
        Invoke(nameof(Hide), lifeTime);

        // Nếu có Animator
        // GetComponent<Animator>().SetTrigger("Up");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _spawner.AddScore(1);
        Hide();
    }
    void Hide()
    {
        CancelInvoke();

        // Animator Down nếu có
        // GetComponent<Animator>().SetTrigger("Down");

        if (_spawner != null)
            _spawner.ReleaseSlot(_slotIndex);

        gameObject.SetActive(false);
    }
}