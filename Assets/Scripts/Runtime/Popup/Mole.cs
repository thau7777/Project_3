using UnityEngine;

public class Mole : MonoBehaviour
{
    public float showTime = 1.5f;
    private Vector3 hidePos;
    private Vector3 showPos;

    void Start()
    {
        hidePos = transform.position + Vector3.down * 2f;
        showPos = transform.position;
        transform.position = hidePos;

        InvokeRepeating("ShowMole", 1f, 2f);
    }

    void ShowMole()
    {
        transform.position = showPos;
        Invoke("HideMole", showTime);
    }

    void HideMole()
    {
        transform.position = hidePos;
    }

    void OnMouseDown()
    {
        Debug.Log("Mole Hit!");
        HideMole();
    }


}
