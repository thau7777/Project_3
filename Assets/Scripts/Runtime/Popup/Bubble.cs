using System.Collections;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Animator animator;
    [SerializeField] private string poolTag;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private RectTransform rt;

    private void Start()
    {
        speed = (int)Random.Range(5f, 15f);
        rt = GetComponent<RectTransform>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        rt.anchoredPosition += Vector2.up * speed * Time.deltaTime;
        MoveFloating();
    }
    void MoveFloating()
    {
        float t = Time.time + speed;

        rb.linearVelocity = new Vector2(
            Mathf.Sin(t) * 8f,
            Mathf.Cos(t * 15f) * 2f
        );
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name.Contains("Water"))
        {
            animator.Play("Bubble_pop");
            StartCoroutine(ReturnToPool());
        }
    }
    IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(1f);
        PoolManager.Instance.Despawn(poolTag, gameObject);
    }
}
