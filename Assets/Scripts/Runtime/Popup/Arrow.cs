using UnityEngine;


public enum ArrowType
{
    Left,
    Up,
    Down,
    Right
}
public class Arrow : MonoBehaviour
{
    public KeyType type;
    public float speed = 300f;
    public HitLine hitLine;

    void Update()
    {
        //transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (Input.GetKeyDown(GetKey()))
        {
            CheckHit();
        }
    }

    KeyCode GetKey()
    {
        switch (type)
        {
            case KeyType.Left: return KeyCode.LeftArrow;
            case KeyType.Down: return KeyCode.DownArrow;
            case KeyType.Up: return KeyCode.UpArrow;
            case KeyType.Right: return KeyCode.RightArrow;
        }
        return KeyCode.None;
    }

    void CheckHit()
    {
        float distance = Mathf.Abs(transform.position.y - hitLine.yPos);

        if (distance < 30f)
        {
            Debug.Log("Perfect");
            Destroy(gameObject);
        }
        else if (distance < 60f)
        {
            Debug.Log("Good");
            Destroy(gameObject);
        }
    }
}
