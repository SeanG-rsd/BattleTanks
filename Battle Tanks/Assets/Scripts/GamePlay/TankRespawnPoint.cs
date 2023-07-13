using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankRespawnPoint : MonoBehaviour
{

    [SerializeField] private float spawnRadius;
    [SerializeField] private int teamIndex;
    private void Awake()
    {
        Tank.OnRespawn += HandleRespawnTank;
    }

    private void OnDestroy()
    {
        Tank.OnRespawn -= HandleRespawnTank;
    }

    private void HandleRespawnTank(Tank tank)
    {
        if (tank.teamIndex == teamIndex)
        {
            float minX = transform.position.x - spawnRadius;
            float minZ = transform.position.z - spawnRadius;

            float maxX = transform.position.x + spawnRadius;
            float maxZ = transform.position.z + spawnRadius;

            Vector3 position = new Vector3(Random.Range(minX, maxX), tank.gameObject.transform.position.y, Random.Range(minZ, maxZ));

            tank.transform.position = position;

            Debug.Log("respawned tank");
        }
    }
}
