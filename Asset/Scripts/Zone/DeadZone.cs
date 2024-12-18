using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private GameManager _demoManager;

    private void Start()
    {
        _demoManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _demoManager.LoadPreviousLevel();
        }
    }
}
