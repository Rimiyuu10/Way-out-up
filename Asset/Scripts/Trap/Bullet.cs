using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameManager demoManager;
    private Vector2 direction;

    [SerializeField] private float rotationSpeed;
    [SerializeField] private float speed;
    [SerializeField] private GameObject impactEffectPrefab;

    [SerializeField] private bool breakPlatform;

    private void Start()
    {
        demoManager = FindObjectOfType<GameManager>();

        Destroy(gameObject, 3);
    }

    public void SetDirection(Vector2 direction)
    {
        this.direction = direction.normalized;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            demoManager.LoadPreviousLevel();
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Ground"))
        {
            if(!breakPlatform)
            {
                FXBreak();
                Destroy(gameObject);
            }
        }
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void FXBreak()
    {
        GameObject fx = Instantiate(impactEffectPrefab, transform.position, transform.rotation);
        Destroy(fx, 1);
    }
}
