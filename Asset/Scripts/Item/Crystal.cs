using UnityEngine;

public class Crystal : MonoBehaviour
{
    
    [SerializeField] private float timeReset;

    private bool playerInZone = false;
    private bool playerClick = false;
    private bool reset = false;

    private PlayerMovement playerScript;
    private SpriteRenderer spriteRend;
    private Animator animator;

    public bool PlayerInZone => playerInZone; // Expose playerInZone as a public property

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerScript = player.GetComponent<PlayerMovement>();
        }

        spriteRend = GetComponentInChildren<SpriteRenderer>();
        animator = spriteRend.GetComponent<Animator>();
    }

    void Update()
    {
        if (playerInZone && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.J)))
        {
            PerformJump();
        }

        Animation();
    }

    public void PerformJump()
    {
        if (playerScript != null)
        {
            playerScript.ResetJumps();
            playerScript.OnJumpInput();

            playerClick = true;
            reset = false;
            Invoke("DeactivateCrystal", 1f);
        }
    }

    private void Animation()
    {
        animator.SetBool("playerClick", playerClick);
        animator.SetBool("playerInZone", playerInZone);
        animator.SetBool("reset", reset);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = false;
        }
    }

    private void DeactivateCrystal()
    {
        gameObject.SetActive(false);
        Invoke("ReactivateCrystal", timeReset);
    }

    private void ReactivateCrystal()
    {
        playerClick = false;
        reset = true;
        gameObject.SetActive(true);
    }
}
