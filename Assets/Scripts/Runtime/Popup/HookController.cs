using UnityEngine;

public class HookController : MonoBehaviour
{
    public Rigidbody2D rb;

    [Header("Throw")]
    public float maxForce = 12f;
    public float chargeSpeed = 8f;

    [Header("Water")]
    public float waterDrag = 3f;
    public float waterGravityScale = 0.5f;

    private float charge;
    private bool charging;
    private bool inWater;
    private bool isFishing;

    private FishItem hookedItem;

    public void Update()
    {
        if (isFishing) return;

        if (Input.GetKey(KeyCode.Space))
        {
            charging = true;
            charge += chargeSpeed * Time.deltaTime;
            charge = Mathf.Clamp(charge, 0f, maxForce);
        }
        if (charging && Input.GetKeyUp(KeyCode.Space))
        {
            charging = false;
            Throw();
        }
    }

    public void Throw()
    {
        Vector2 force = transform.up * charge;
        rb.AddForce(force, ForceMode2D.Impulse);
        charge = 0f;
    }
    /*void Throw()
    {
        rb.gravityScale = 0f;
        rb.drag = 0f;
        rb.velocity = new Vector2(charge, charge * 0.6f);
        charge = 0f;
    }*/

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Cham mat nuoc
        if(!inWater && collision.gameObject.name.Contains("Water"))
        { 
            Debug.Log("Da cham nuoc");
            inWater = true;
            rb.drag = waterDrag;
            rb.gravityScale = waterGravityScale;
        }

        FishItem item = collision.GetComponent<FishItem>();
        if(item != null && !isFishing)
        {
            hookedItem = item;
            StartFishing();
        }
    }

    public void StartFishing()
    {
        isFishing = true;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        FishingUI.instance.StartFishing(this);
    }
    public void PullUp(bool success)
    {
        isFishing = false;
        rb.isKinematic = false;
        rb.gravityScale = 0f;
        rb.drag = 0f;

        if(success && hookedItem !=null)
        {
            hookedItem.AttachToHook(transform);
        }

        rb.velocity = new Vector2(-5f, 7f);
    }
}
