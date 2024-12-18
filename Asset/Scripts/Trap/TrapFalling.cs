using UnityEngine;

public class TrapFalling : MonoBehaviour
{
    private GameManager demoManager;
    [SerializeField] private float speed;              // Vận tốc rơi xuống
    [SerializeField] private float delayBeforeMoving;  // Thời gian trì hoãn trước khi rơi
    [SerializeField] private GameObject impactEffectPrefab;
    [SerializeField] private Transform impactPoint;    // Điểm va chạm để kiểm tra với mặt đất

    private bool canMove = false;

    private void Start()
    {
        demoManager = FindObjectOfType<GameManager>();
        // Sau một khoảng thời gian, cho phép bẫy rơi xuống
        Invoke(nameof(EnableMovement), delayBeforeMoving);

        // Hủy đối tượng sau 10 giây (hoặc tùy chỉnh thời gian phù hợp)
        Destroy(gameObject, 10);
    }

    private void Update()
    {
        if (canMove)
        {
            // Di chuyển GameObject xuống dưới theo trục y với tốc độ speed
            transform.Translate(Vector2.up * speed * Time.deltaTime);

            // Kiểm tra xem impactPoint có chạm vào ground hay không
            RaycastHit2D hit = Physics2D.Raycast(impactPoint.position, Vector2.down, 0.1f);
            if (hit.collider != null && hit.collider.CompareTag("Ground"))
            {
                FXBreak();
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            demoManager.LoadPreviousLevel();
            Destroy(gameObject);
        }
    }

    private void EnableMovement()
    {
        canMove = true;  // Cho phép bẫy rơi xuống sau khi hết thời gian trì hoãn
    }

    private void FXBreak()
    {
        // Tạo hiệu ứng va chạm tại vị trí impactPoint
        GameObject fx = Instantiate(impactEffectPrefab, impactPoint.position, impactPoint.rotation);
        Destroy(fx, 1);
    }
}
