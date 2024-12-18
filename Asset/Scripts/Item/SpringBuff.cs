using UnityEngine;

public class SpringBuff : MonoBehaviour
{
    private PlayerMovement playerScript;
    private bool playerInZone = false;

    public float streng;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerScript = player.GetComponent<PlayerMovement>();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerInZone = true;
        PerformJump();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        playerInZone = false;
    }

    public void PerformJump()
    {
        if (playerScript != null && playerInZone)
        {
            playerScript.ResetJumps();
            playerScript.JumpBuff();
        }
    }
}
