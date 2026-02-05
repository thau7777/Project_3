using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Thêm nếu bạn cần đổi màu/hình ảnh

public class MoleUI : MonoBehaviour, IPointerClickHandler
{
    private Image _image; // Ví dụ nếu bạn muốn reset màu sắc

    void Awake()
    {
        _image = GetComponent<Image>();
    }

    // Hàm này tự động gọi mỗi khi bạn SetActive(true)
    void OnEnable()
    {
        ResetMole();
    }

    void ResetMole()
    {
        // Reset lại mọi thứ về trạng thái ban đầu
        transform.localScale = Vector3.one;
        if (_image != null) _image.color = Color.white;

        // Nếu bạn có dùng Animation, hãy Play lại animation IDLE ở đây
        // GetComponent<Animator>().Play("Idle", 0, 0);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Đập trúng chuột UI!");

        // Thay vì biến mất ngay, bạn có thể chạy animation chết
        // Sau đó gọi hàm này để trả về Pool:
        gameObject.SetActive(false);
    }
}