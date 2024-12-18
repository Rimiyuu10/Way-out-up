using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerGun : MonoBehaviour
{
    private GameManager demoManager;

    [SerializeField] private Transform fireWarningPoint;
    [SerializeField] private Transform fireLaserPoint;
    [SerializeField] private float speedRotate;
    [SerializeField] private float fireInterval;

    private Transform player;
    private float nextFireTime;

    [Header("Use Laser")]
    [SerializeField] private bool useLaser = false;
    [SerializeField] private GameObject lineWarningObject;
    [SerializeField] private List<GameObject> lineLaserObjects;
    private LineRenderer lineWarning;
    private List<LineRenderer> lineLasers;
    [Space]
    [SerializeField] private GameObject endVFXPrefab;
    [Space]
    [SerializeField] private float timeToFire;
     public float timeFireOff;
    [Space]
    [SerializeField] public AnimationCurve curve;

    private GameObject endVFX;

    public bool noShake;

    private void Start()
    {
        demoManager = FindObjectOfType<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        nextFireTime = Time.time;

        lineWarning = lineWarningObject.GetComponent<LineRenderer>();

        lineLasers = new List<LineRenderer>();
        foreach (GameObject lineLaserObject in lineLaserObjects)
        {
            lineLasers.Add(lineLaserObject.GetComponent<LineRenderer>());
        }

        // Chỉ tạo một endVFX
        endVFX = Instantiate(endVFXPrefab, fireLaserPoint.position, Quaternion.identity);
        endVFX.transform.SetParent(demoManager.GetCurrentLevelTransform(), false);
        endVFX.SetActive(false);

        lineWarningObject.SetActive(false);
        foreach (GameObject lineLaserObject in lineLaserObjects)
        {
            lineLaserObject.SetActive(false);
        }

        if (useLaser)
        {
            StartCoroutine(SwitchLines());
        }
    }

    private void Update()
    {
        RotateTowardsPlayer();

        if (useLaser && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireInterval + timeFireOff;
        }

        if (lineWarningObject.activeSelf)
        {
            UpdateLineRenderer(lineWarning, fireWarningPoint);
        }
        else
        {
            for (int i = 0; i < lineLasers.Count; i++)
            {
                if (lineLaserObjects[i].activeSelf)
                {
                    UpdateLineRenderer(lineLasers[i], fireLaserPoint);
                }
            }
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

    private void UpdateLineRenderer(LineRenderer lineRenderer, Transform firePoint)
    {
        lineRenderer.SetPosition(0, firePoint.position);
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right);

        if (hit.collider != null)
        {
            lineRenderer.SetPosition(1, hit.point);

            if (hit.collider.CompareTag("Player") && lineRenderer == lineLasers[0])
            {
                StartCoroutine(WaitAndLoadLevel());
            }

            UpdateVFX(endVFX, hit.point, firePoint.rotation);
        }
        else
        {
            Vector3 endPosition = firePoint.position + firePoint.right * 100f;
            lineRenderer.SetPosition(1, endPosition);

            UpdateVFX(endVFX, endPosition, firePoint.rotation);
        }
    }

    private IEnumerator WaitAndLoadLevel()
    {
        yield return new WaitForSeconds(0.1f); // Thêm khoảng trễ trước khi load level
        demoManager.LoadPreviousLevel();
    }

    private void UpdateVFX(GameObject vfx, Vector3 position, Quaternion rotation)
    {
        vfx.transform.position = position;
        vfx.transform.rotation = rotation;
    }

    private IEnumerator SwitchLines()
    {
        while (true)
        {
            UpdateLineRenderer(lineWarning, fireWarningPoint);

            lineWarningObject.SetActive(true);
            foreach (GameObject lineLaserObject in lineLaserObjects)
            {
                lineLaserObject.SetActive(false);
            }
            yield return new WaitForSeconds(timeToFire);

            for (int i = 0; i < lineLasers.Count; i++)
            {
                UpdateLineRenderer(lineLasers[i], fireLaserPoint);
            }

            lineWarningObject.SetActive(false);
            for (int i = 0; i < lineLaserObjects.Count; i++)
            {
                lineLaserObjects[i].SetActive(true);
                endVFX.SetActive(true);

                if (!noShake)
                {
                    CameraShake.MyInstance.StartShake(timeFireOff, curve);
                }
            }
            yield return new WaitForSeconds(timeFireOff);

            endVFX.SetActive(false);
        }
    }

    public void DisableEndVFXs()
    {
        if (endVFX != null)
        {
            endVFX.SetActive(false);
        }
    }
}
