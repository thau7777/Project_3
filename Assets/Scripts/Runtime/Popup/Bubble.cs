using System.Collections;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Animator animator;
    [SerializeField] private string poolTag;
    [SerializeField] private RectTransform rtb;

    private void Start()
    {
        speed = (int)Random.Range(20f, 25f);
        rtb = GetComponent<RectTransform>();
    }
    private void Update()
    {
        
        rtb.anchoredPosition += Vector2.up * speed * Time.deltaTime;
        MoveFloating();
    }
    void MoveFloating()
    {
        float t = Time.time + speed;

        rtb.anchoredPosition += new Vector2(
            Mathf.Sin(t) * 8f,
            Mathf.Cos(t * 15f) * 2f
        ) * Time.deltaTime;
    }
    public void UpdateRT(RectTransform rect)
    {
        rtb.anchoredPosition = rect.anchoredPosition;
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name.Contains("Water"))
        {
            
            StartCoroutine(ReturnToPool());
        }
    }
    IEnumerator ReturnToPool()
    {
        
        yield return new WaitForSeconds(1f);
        PoolManager.Instance.Despawn(poolTag, gameObject);
    }
}
