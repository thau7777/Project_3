using UnityEngine;
using UnityEngine.UI;

public class HookController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    [SerializeField] private Image chargeBar;
    public GameObject powerBar;

    [Header("Charge")]
    public float chargeSpeed = 1.5f;

    [Header("Throw Force")]
    public float minForce = 3f;
    public float maxForce = 15f;

    [Header("Water Physics")]
    public float waterDrag = 3f;
    public float waterGravity = 6f;

    [Header("Normal Physics")]
    public float normalMass = 0.5f;
    public float normalGravity = 10f;

    // ===== State =====
    private bool isCharging;
    private bool hasThrown;
    private bool inWater;
    private FishItem hookedItem;
    private Vector2 initialPosition;

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

    // ================= INPUT =================
    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // SPACE lần 1 → bắt đầu charge
            if (!isCharging && !hasThrown)
            {
                isCharging = true;
                powerBar.SetActive(true);
            }
            // SPACE lần 2 → ném hook
            else if (isCharging)
            {
                isCharging = false;
                powerBar.SetActive(false);

                ThrowHook(chargeBar.fillAmount);
            }
        }
        if(Input.GetKey(KeyCode.R) && inWater)
        {
            ResetHook();
        }
    }

    // ================= CHARGE BAR =================
    void UpdateChargeBar()
    {
        if (!isCharging) return;

        chargeBar.fillAmount =
            Mathf.PingPong(Time.time * chargeSpeed, 1f);
    }

    // ================= THROW =================
    void ThrowHook(float charge01)
    {
        hasThrown = true;

        rb.mass = normalMass; // nhẹ hơn để bay xa hơn
        rb.gravityScale = normalGravity; // rơi nhanh hơn
        float force = Mathf.Lerp(minForce, maxForce, charge01);

        rb.linearVelocity = Vector2.zero;

        // lực bắn ngang
        rb.AddForce(Vector2.right * force * 15f, ForceMode2D.Impulse);

        // lực nâng lên (ít hơn)
        rb.AddForce(Vector2.up * force * 5f, ForceMode2D.Impulse);
    }

    // ================= COLLISION =================
    void OnTriggerEnter2D(Collider2D other)
    {
        // Chạm mặt nước
        if (!inWater && other.gameObject.name.Contains("Water"))
        {
            inWater = true;
            rb.linearDamping = waterDrag;
            rb.gravityScale = waterGravity;
        }

        // Chạm cá / rác
        FishItem item = other.GetComponent<FishItem>();
        if (item != null)
        {
            hookedItem = item;
            
            rb.linearVelocity = Vector2.zero;

            FishingUI.instance.StartFishing(this);
            rb.gravityScale = 0f;
        }
    }

    // ================= PULL UP =================
    public void PullUp(bool success)
    {
        rb.gravityScale = 0f;
        rb.linearDamping = 0f;

        if (success && hookedItem != null)
        {
            hookedItem.AttachToHook(transform);
        }

        rb.linearVelocity = new Vector2(-5f, 7f);

        ResetHook();
    }

    void ResetHook()
    {
        transform.position = initialPosition;
        hasThrown = false;
        inWater = false;
        hookedItem = null;
        rb.mass = normalMass;
        rb.gravityScale = normalGravity;
        rb.linearDamping = 0f;
        rb.linearVelocity = Vector2.zero;
    }
}