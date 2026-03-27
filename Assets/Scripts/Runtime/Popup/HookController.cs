using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class HookController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    [SerializeField] private Image chargeBar;
    public GameObject powerBar;
    public TutorialTrigger fishTutorial;
    [SerializeField] private List<GameObject> image;

    [Header("Charge")]
    public float chargeSpeed = 1.5f;

    [Header("Throw Force")]
    public float minForce = 3f;
    public float maxForce = 15f;

    [Header("Water Physics")]
    public float waterDrag = 1.5f;
    public float waterGravity = 10f;

    [Header("Normal Physics")]
    public float normalMass = 0.5f;
    public float normalGravity = 10f;


    private bool isCharging;
    private bool hasThrown;
    private bool inWater;
    private bool OnGround;
    private FishItem hookedItem;
    private Vector2 initialPosition;
    private float chargeStartTime;

    void Start()
    {
        chargeBar.fillAmount = 0f;
        powerBar.SetActive(false);
        initialPosition = transform.position;
    }

    void Update()
    {
        HandleInput();
        UpdateChargeBar();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // SPACE lần 1 → bắt đầu charge
            if (!isCharging && !hasThrown)
            {
                isCharging = true;
                powerBar.SetActive(true);

                chargeStartTime = Time.time;
            }
            // SPACE lần 2 → ném hook
            else if (isCharging)
            {
                isCharging = false;
                powerBar.SetActive(false);

                ThrowHook(chargeBar.fillAmount);
            }
        }
        if(Input.GetKey(KeyCode.R) && OnGround)
        {
            ResetHook();
        }
    }

    void UpdateChargeBar()
    {
        if (!isCharging) return;

        float t = (Time.time - chargeStartTime) * chargeSpeed;

        chargeBar.fillAmount = Mathf.PingPong(t + 0.5f, 1f);
    }

    void ThrowHook(float charge01)
    {
        hasThrown = true;

        rb.mass = normalMass; // nhẹ hơn để bay xa hơn
        rb.gravityScale = normalGravity; // rơi nhanh hơn
        float force = Mathf.Lerp(minForce, maxForce, charge01);

        rb.linearVelocity = Vector2.zero;

        rb.AddForce(Vector2.right * force * 15f, ForceMode2D.Impulse);

        rb.AddForce(Vector2.up * force * 5f, ForceMode2D.Impulse);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Chạm mặt nước
        if (!inWater && other.gameObject.name.Contains("Water"))
        {
            inWater = true;
            rb.linearDamping = waterDrag;
            rb.gravityScale = waterGravity;
        }
        

        if (!other.TryGetComponent(out FishItem item)) return;
        if (item.state != FishState.Swimming) return;

        hookedItem = item;
        item.OnHooked();
        image.ForEach(i => i.SetActive(true));

        fishTutorial.Trigger();

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.simulated = false;

        FishingUI.instance.StartFishing(this);
        StartCoroutine(DelayAction(1f, () => image.ForEach(i => i.SetActive(false))));
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            OnGround = true;
        }
    }
    public void PullUp(bool success)
    {
        rb.linearVelocity = Vector2.zero;
        

        if (hookedItem != null)
        {
            if (success)
                hookedItem.OnCaught();
            else
                hookedItem.OnEscape();
        }

        ResetHook();
    }

    void ResetHook()
    {
        transform.position = initialPosition;
        hasThrown = false;
        inWater = false;
        OnGround = false;
        hookedItem = null;
        rb.mass = normalMass;
        rb.gravityScale = normalGravity;
        rb.linearDamping = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = true;
    }
    private IEnumerator DelayAction(float delay, System.Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}