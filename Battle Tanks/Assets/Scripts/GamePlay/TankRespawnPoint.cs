using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankRespawnPoint : MonoBehaviour
{

    [SerializeField] private float spawnRadius;
    [SerializeField] private int teamIndex;

    [SerializeField] private GameObject respawnCam;
    private void Awake()
    {
        Tank.OnRespawn += HandleRespawnTank;
        Tank.OnAlive += HandleTankAlive;
        Tank.OnStart += HandleStartGame;
        Tank.OnStarted += HandleGameStarted;
    }

    private void OnDestroy()
    {
        Tank.OnRespawn -= HandleRespawnTank;
        Tank.OnAlive -= HandleTankAlive;
        Tank.OnStart -= HandleStartGame;
        Tank.OnStarted -= HandleGameStarted;
    }

    private void HandleRespawnTank(Tank tank)
    {
        Debug.Log("a tank has been respawned");
        if (tank.teamIndex == teamIndex)
        {
            
            float minX = transform.position.x - spawnRadius;
            float minZ = transform.position.z - spawnRadius;

            float maxX = transform.position.x + spawnRadius;
            float maxZ = transform.position.z + spawnRadius;

            Vector3 position = new Vector3(Random.Range(minX, maxX), tank.gameObject.transform.position.y, Random.Range(minZ, maxZ));

            tank.transform.position = position;

            respawnCam.SetActive(true);
        }
    }

    private void HandleTankAlive(Tank tank)
    {
        if (tank.teamIndex == teamIndex)
        {
            respawnCam.SetActive(false);
        }
    }

    private void HandleStartGame(Tank tank)
    {
        if (tank.teamIndex == teamIndex)
        {
            respawnCam.SetActive(true);
        }
    }

    private void HandleGameStarted(Tank tank)
    {
        Debug.Log("cam turned off");
        if (tank.teamIndex == teamIndex)
        {
            respawnCam.SetActive(false);
        }
    }
}
