using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 300f;
    public KeyCode key;
    public HitLine hitLine;

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (Input.GetKeyDown(key))
        {
            CheckHit();
        }
    }

    void CheckHit()
    {
        float distance = Mathf.Abs(transform.position.y - hitLine.yPos);

        if (distance < 30f)
        {
            Debug.Log("Perfect!");
            Destroy(gameObject);
        }
        else if (distance < 60f)
        {
            Debug.Log("Good!");
            Destroy(gameObject);
        }
    }
}
