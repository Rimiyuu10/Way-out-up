using UnityEngine;

public class TurretGun : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float speedRotate;
    [SerializeField] private float fireInterval;
    [SerializeField] private GameObject fireEffectPrefab;
    [SerializeField] private AnimationCurve curve;

    [SerializeField] private bool noShake;

    private Transform player;
    private float nextFireTime;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        nextFireTime = Time.time;
    }

    private void Update()
    {
        RotateTowardsPlayer();

        if (Time.time >= nextFireTime)
        {
            FireBullet();
            nextFireTime = Time.time + fireInterval;
        }
    }

    private void RotateTowardsPlayer()
    {
        if (player != null)
        {
            Vector2 direction = player.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speedRotate * Time.deltaTime);
        }
    }

    private void FireBullet()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            if (!noShake) 
            {
                CameraShake.MyInstance.StartCoroutine(CameraShake.MyInstance.Shake(0.3f, curve));
            }

            GameObject fxFire = Instantiate(fireEffectPrefab, firePoint.position, firePoint.rotation);
            Destroy(fxFire, 1);
            Bullet bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.SetDirection(firePoint.right);
        }
    }
}
