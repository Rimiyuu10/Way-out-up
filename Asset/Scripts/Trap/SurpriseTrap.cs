using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurpriseTrap : MonoBehaviour
{
    [SerializeField] private float size = 5f;
    [SerializeField] private GameObject objectSpawn;
    [SerializeField] private Transform pointInstantiateTrap;
    private PlayerMovement player;  // Tham chiếu tới người chơi

    private bool hasTriggered = false;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (!hasTriggered && player.transform.position.x > transform.position.x)
        {
            SpawnTrap();
            hasTriggered = true;
        }
    }

    private void SpawnTrap()
    {
        Instantiate(objectSpawn, pointInstantiateTrap.position, pointInstantiateTrap.rotation);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * size);
    }
}
