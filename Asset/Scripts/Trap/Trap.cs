using UnityEngine;

public class Trap : MonoBehaviour
{
    private GameManager demoManager;

    private void Start()
    {
        demoManager = FindObjectOfType<GameManager>();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            demoManager.LoadPreviousLevel();
        }
    }
}
