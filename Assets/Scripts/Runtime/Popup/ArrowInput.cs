using System.Collections.Generic;
using UnityEngine;

public class ArrowInput : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ArrowManager arrowManager;

    [Header("Timing")]
    public float perfectTime = 0.2f;
    public float goodTime = 0.4f;
    private float appearTime;


    private Dictionary<KeyCode, ArrowType> keyMap;


    private void Start()
    {
         arrowManager = FindAnyObjectByType<ArrowManager>();
        keyMap = new Dictionary<KeyCode, ArrowType>()
        {
            { KeyCode.LeftArrow,  ArrowType.Left },
            { KeyCode.UpArrow,    ArrowType.Up },
            { KeyCode.DownArrow,  ArrowType.Down },
            { KeyCode.RightArrow, ArrowType.Right }
        };


    }
    void Update()
    {
        foreach (var key in keyMap)
        {
            if (Input.GetKeyDown(key.Key))
            {
                CheckInput(key.Value);
                break;
            }
        }
    }

    public void CheckInput(ArrowType input)
    {
        Debug.Log("Arrow Type now: " + arrowManager.currentArrow);
        float deltaTime = Time.time - appearTime;

        Debug.Log("Input Arrow Type: " + input);

        if (input == arrowManager.currentArrow)
        {
            if (deltaTime <= perfectTime)
            {
                Debug.Log("PERFECT!");
            }
            else if (deltaTime <= goodTime)
            {
                Debug.Log("GOOD!");
            }
            else
            {
                Debug.Log("BAD!");
            }
            arrowManager.NextArrow();
        }
        else
        {
            Debug.Log("MISS!");
        }
    }

    //void NextArrow()
    //{
    //    currentArrow = (ArrowType)Random.Range(0, 4);
    //    Debug.Log("Next: " + currentArrow);

    //}
}
