using UnityEngine;

public class BreakPlatform : MonoBehaviour
{
    [SerializeField] private GameObject fxBreak;

    [SerializeField] private float timeToBreak;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Invoke("Break", timeToBreak);
        }
    }

    private void Break()
    {
        Destroy(gameObject);

        GameObject fx = Instantiate(fxBreak, transform.position, transform.rotation);
        Destroy(fx, 2);
    }
}
