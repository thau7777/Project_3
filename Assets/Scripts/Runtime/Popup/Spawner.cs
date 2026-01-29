using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform canvas;
    public HitLine hitLine;

    void Start()
    {
        InvokeRepeating(nameof(SpawnArrow), 1f, 1f);
    }

    void SpawnArrow()
    {
        GameObject arrowObj = Instantiate(arrowPrefab, canvas);
        Arrow arrow = arrowObj.GetComponent<Arrow>();

        arrow.hitLine = hitLine;

        RectTransform rt = arrowObj.GetComponent<RectTransform>();
        int rand = Random.Range(0, 4);

        switch (rand)
        {
            case 0:
                rt.anchoredPosition = new Vector2(-150, 400);
                arrow.key = KeyCode.LeftArrow;
                break;
            case 1:
                rt.anchoredPosition = new Vector2(-50, 400);
                arrow.key = KeyCode.DownArrow;
                break;
            case 2:
                rt.anchoredPosition = new Vector2(50, 400);
                arrow.key = KeyCode.UpArrow;
                break;
            case 3:
                rt.anchoredPosition = new Vector2(150, 400);
                arrow.key = KeyCode.RightArrow;
                break;
        }
    }
}
