using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class TankRespawnPoint : MonoBehaviour
{

    [SerializeField] private float spawnRadius;
    public int teamIndex;

    [SerializeField] private GameObject respawnCam;
    private void Awake()
    {
        Tank.OnRespawn += HandleRespawnTank;
        Tank.OnAlive += HandleTankAlive;
        Tank.OnBeginGame += HandleStartGame;
        Tank.OnStarted += HandleGameStarted;
        Tank.OnNewRound += HandleRespawnTank;
    }

    private void OnDestroy()
    {
        Tank.OnRespawn -= HandleRespawnTank;
        Tank.OnAlive -= HandleTankAlive;
        Tank.OnBeginGame -= HandleStartGame;
        Tank.OnStarted -= HandleGameStarted;
        Tank.OnNewRound -= HandleRespawnTank;
    }

    private void HandleRespawnTank(Tank tank)
    {
        Debug.Log("a tank has been respawned");
        if (tank.teamIndex == teamIndex)
        {

            Vector2 pos = GetPoint();

            Vector3 position = new Vector3(pos.x, tank.gameObject.transform.position.y, pos.y);

            tank.transform.position = position;

            respawnCam.SetActive(true);
        }
    }

    public Vector2 GetPoint()
    {
        float minX = transform.position.x - spawnRadius;
        float minZ = transform.position.z - spawnRadius;

        float maxX = transform.position.x + spawnRadius;
        float maxZ = transform.position.z + spawnRadius;

        return new Vector2(Random.Range(minX, maxX), Random.Range(minZ, maxZ));
    }

    private void HandleTankAlive(Tank tank)
    {
        if (tank.teamIndex == teamIndex)
        {
            respawnCam.SetActive(false);
        }
    }

    private void HandleStartGame(Tank tank, Player player)
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
