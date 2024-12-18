using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public PlayerData Data;

    [SerializeField] private float speed;
    public GameObject ways;
    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private bool stopMoving;  // Biến để xác định có dừng lại ở điểm cuối hay không

    private int currentPointIndex = 0;
    private Vector3 targetPos;

    private GameManager gameManager;
    PlayerMovement player;
    Rigidbody2D rb;
    Vector3 movementDirection;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();

        wayPoints = new Transform[ways.transform.childCount];
        for (int i = 0; i < ways.gameObject.transform.childCount; i++)
        {
            wayPoints[i] = ways.transform.GetChild(i).gameObject.transform;
        }

        transform.position = wayPoints[0].position;
        targetPos = wayPoints[0].position;

        DirectionCalculate();
    }

    private void Update()
    {
        MovePlatform();
    }

    private void FixedUpdate()
    {
        rb.velocity = movementDirection * speed;

        if (player.isOnPlatform && player.IsCrushed())
        {
            gameManager.LoadPreviousLevel();
        }

        // Nếu platform đã dừng lại thì reset vận tốc về 0
        if (stopMoving && currentPointIndex >= wayPoints.Length)
        {
            rb.velocity = Vector2.zero;
            movementDirection = Vector3.zero;
        }
    }


    private void MovePlatform()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPos) < .25f)
        {
            currentPointIndex++;

            if (stopMoving && currentPointIndex >= wayPoints.Length)
            {
                speed = 0; // Dừng hẳn platform
                return;
            }

            if (currentPointIndex >= wayPoints.Length)
            {
                currentPointIndex = 0;
            }

            targetPos = wayPoints[currentPointIndex].position;

            if (speed > 0)
            {
                DirectionCalculate();
            }
        }
    }


    private void DirectionCalculate()
    {
        movementDirection = (targetPos - transform.position).normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (player != null)
        {
            if (collision.CompareTag("Player"))
            {
                player.isOnPlatform = true;
                player.platformRb = rb;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (player != null)
        {
            if (collision.CompareTag("Player"))
            {
                player.isOnPlatform = false;
                player.platformRb = null;
            }
        }
    }
}
