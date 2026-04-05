using System.Collections;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Animator animator;
    [SerializeField] private string poolTag;
    [SerializeField] private RectTransform rtb;

    private float internalTimeOffset;

    private void Awake()
    {
        rtb = GetComponent<RectTransform>();
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        speed = (int)Random.Range(40f, 45f);
        internalTimeOffset = Random.Range(0, 100);
    }
    private void Update()
    {
        
        rtb.anchoredPosition += Vector2.up * speed * Time.deltaTime;
        MoveFloating();
    }
   
    void MoveFloating()
    {
        float t = Time.time + internalTimeOffset;
        rtb.anchoredPosition += new Vector2(
            Mathf.Sin(t * 2) * 15f,
            Mathf.Cos(t * 2) * 5f
        ) * Time.deltaTime;
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
