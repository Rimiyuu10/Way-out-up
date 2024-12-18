using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private Transform destination;
    private PlayerMovement player;
    private Animator animator;
    private Rigidbody2D rb;

    private bool portalIn;
    private bool portalOut;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        animator = GameObject.FindWithTag("Player Animator").GetComponent<Animator>();
        rb = player.GetComponent<Rigidbody2D>();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(Vector2.Distance(player.transform.position, transform.position) > 0.3f)
            {
                StartCoroutine(PortalIn());
            }
        }
    }

    private IEnumerator PortalIn()
    {
        rb.simulated = false;
        animator.Play("Portal In");
        StartCoroutine(MoveInPortal());
        yield return new WaitForSeconds(0.5f);
        player.transform.position = destination.position;
        rb.velocity = Vector2.zero;
        animator.Play("Portal Out");
        yield return new WaitForSeconds(0.5f);
        rb.simulated = true;
    }

    private IEnumerator MoveInPortal() // Tạo hiệu ứng di chuyển đến trung tâm tele
    {
        float timer = 0;
        while (timer < 0.5f) 
        {
            player.transform.position = Vector2.MoveTowards(player.transform.position, transform.position, 3 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
    }
}
