using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public EnemyData enemyData;
    private GameManager gameManager; // Thêm biến tham chiếu tới GameManager
    private GameObject player;

    #region COMPONENTS
    public Rigidbody2D RB { get; private set; }
    public EnemyAnimator enemyAnimator { get; private set; }
    #endregion

    #region STATE PARAMETERS
    public bool isMovingRight { get; private set; }

    private bool isWaiting = false;
    private bool isGround = true;
    public bool playerDetected { get; private set; }

    private float timeSincePlayerOutOfLineRenderer = 0f;
    #endregion

    #region CHECK PARAMETERS
    [Header("Check")]
    [SerializeField] private Transform wallGroundRight;
    [SerializeField] private Transform checkGroundRight;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform firePoint;

    [Header("Layer")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask detectionLayer;

    [Header("Detection")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject[] detectionIndicator;
    [SerializeField] private float vectorY;

    [Header("Size")]
    [SerializeField] private Vector2 wallCheckSize;
    [SerializeField] private Vector2 groundCheckSize;
    #endregion

    private void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<EnemyAnimator>();
        player = GameObject.FindGameObjectWithTag("Player");

        gameManager = FindObjectOfType<GameManager>(); // Tìm và gán GameManager

        StartCoroutine(RandomFlipCoroutine());

        isMovingRight = true;
    }

    private void Update()
    {
        isGround = Physics2D.OverlapBox(groundCheck.position, wallCheckSize, 0, groundLayer);
        HandlePlayerDetection();
        UpdateDetectionIndicators();
    }

    private void FixedUpdate()
    {
        if (!isWaiting)
        {
            if (!playerDetected && IsPlayerInLineRendererRange())
            {
                RB.velocity = Vector2.zero;
            }
            else if (playerDetected)
            {
                if (player.transform.position.y > transform.position.y + vectorY)
                {
                    FastMove();
                }
                else
                {
                    ChasePlayer();
                }
            }
            else
            {
                if (timeSincePlayerOutOfLineRenderer < enemyData.timeMoveNext)
                {
                    timeSincePlayerOutOfLineRenderer += Time.fixedDeltaTime;
                    RB.velocity = Vector2.zero;
                }
                else
                {
                    Move();

                    if (IsHittingWall() || !IsGroundAhead())
                    {
                        StartCoroutine(WaitAndFlip());
                    }
                }
            }
        }

        UpdateLineRenderer(lineRenderer, firePoint);
    }

    private bool IsPlayerInLineRendererRange()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(firePoint.position, firePoint.right, enemyData.detectionDistance, detectionLayer);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                bool isPlayerInFront = (isMovingRight && player.transform.position.x > transform.position.x) || (!isMovingRight && player.transform.position.x < transform.position.x);
                if (hit.collider.gameObject == player && isPlayerInFront)
                {
                    return true;
                }

                if (((1 << hit.collider.gameObject.layer) & groundLayer) != 0)
                {
                    break;
                }
            }
        }
        return false;
    }

    private void Move()
    {
        float moveDirection = isMovingRight ? 1 : -1;
        Vector2 velocity = new Vector2(moveDirection * enemyData.runMaxSpeed, RB.velocity.y);
        RB.velocity = velocity;
    }

    private void ChasePlayer()
    {
        float moveDirection = player.transform.position.x > transform.position.x ? 1 : -1;

        if ((moveDirection > 0 && !isMovingRight) || (moveDirection < 0 && isMovingRight))
        {
            Flip();
        }

        Vector2 velocity = new Vector2(moveDirection * enemyData.runDetected, RB.velocity.y);
        RB.velocity = velocity;
    }
    private void FastMove()
    {
        float moveDirection = isMovingRight ? 1 : -1;
        Vector2 velocity = new Vector2(moveDirection * enemyData.runDetected, RB.velocity.y);
        RB.velocity = velocity;
    }

    private bool IsHittingWall()
    {
        return Physics2D.OverlapBox(wallGroundRight.position, wallCheckSize, 0, groundLayer);
    }

    private bool IsGroundAhead()
    {
        return Physics2D.OverlapBox(checkGroundRight.position, groundCheckSize, 0, groundLayer);
    }

    private void HandlePlayerDetection()
    {
        bool playerHit = IsPlayerInLineRendererRange();

        if (playerHit)
        {
            timeSincePlayerOutOfLineRenderer = 0f;
            enemyData.detectionTimeout = 0f;
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            enemyData.detectionProgress += Time.deltaTime * (enemyData.detectionDistance - distanceToPlayer) / enemyData.detectionDistance;
            if (enemyData.detectionProgress >= enemyData.detectionThreshold)
            {
                playerDetected = true;
                enemyData.detectionProgress = enemyData.detectionThreshold;
            }
        }
        else
        {
            enemyData.detectionTimeout += Time.deltaTime;
            enemyData.detectionProgress -= Time.deltaTime * enemyData.detectionDecreaseSpeed;
            if (enemyData.detectionProgress <= 0f)
            {
                enemyData.detectionProgress = 0f;
            }
            if (enemyData.detectionTimeout >= enemyData.detectionCooldown)
            {
                playerDetected = false;
            }
        }
    }

    private IEnumerator WaitAndFlip()
    {
        if (isGround)
        {
            isWaiting = true;
            RB.bodyType = RigidbodyType2D.Static;

            yield return new WaitForSeconds(enemyData.waitTime);

            RB.bodyType = RigidbodyType2D.Dynamic;
            Flip();
            isWaiting = false;
        }
        else
        {
            RB.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    private void Flip()
    {
        isMovingRight = !isMovingRight;

        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;

        firePoint.localRotation = Quaternion.Euler(0, isMovingRight ? 0 : 180, 0);

        Move();

        if (enemyAnimator != null)
        {
            enemyAnimator.startedFlipping = true;
        }
    }

    private IEnumerator RandomFlipCoroutine()
    {
        while (true)
        {
            float waitTime = Random.Range(enemyData.minFlipTime, enemyData.maxFlipTime);
            yield return new WaitForSeconds(waitTime);

            if (!isWaiting && !playerDetected)
            {
                StartCoroutine(WaitAndFlip());
            }
        }
    }

    private void UpdateDetectionIndicators()
    {
        Color currentColor;
        if (!playerDetected)
        {
            currentColor = Color.Lerp(enemyData.startColor, enemyData.endColor, enemyData.detectionProgress / enemyData.detectionThreshold);
        }
        else
        {
            currentColor = enemyData.endColor;
        }

        foreach (var indicator in detectionIndicator)
        {
            indicator.GetComponent<SpriteRenderer>().color = currentColor;
        }
    }

    private void UpdateLineRenderer(LineRenderer lineRenderer, Transform firePoint)
    {
        lineRenderer.SetPosition(0, firePoint.position);
        RaycastHit2D[] hits = Physics2D.RaycastAll(firePoint.position, firePoint.right, enemyData.detectionDistance, detectionLayer);

        Vector3 endPosition = firePoint.position + firePoint.right * enemyData.detectionDistance;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject != player)
            {
                endPosition = hit.point;
                break;
            }
        }
        lineRenderer.SetPosition(1, endPosition);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && playerDetected)
        {
            gameManager.LoadPreviousLevel();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(wallGroundRight.position, wallCheckSize);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(checkGroundRight.position, groundCheckSize);
    }
}
