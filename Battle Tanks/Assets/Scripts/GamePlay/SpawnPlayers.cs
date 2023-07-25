using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnPlayers : MonoBehaviour
{

    public GameObject[] bluePlayerPrefabs;
    public GameObject[] redPlayerPrefabs;
    GameObject playerToSpawn;

    public TankRespawnPoint[] spawnPoints;

    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;
    public float Y;

    private void Awake()
    {
        MapGeneator.OnMapGenerated += HandleSpawnPlayers;
    }

    private void OnDestroy()
    {
        MapGeneator.OnMapGenerated -= HandleSpawnPlayers;
    }

    private void HandleSpawnPlayers()
    {

    }

    private void Start()
    {
        Vector3 randomPosition = new Vector3(Random.Range(minX, maxX), Y, Random.Range(minZ, maxZ));

        if ((int)PhotonNetwork.LocalPlayer.GetPhotonTeam().Code == 1)
        {
            playerToSpawn = bluePlayerPrefabs[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];

        }
        else if ((int)PhotonNetwork.LocalPlayer.GetPhotonTeam().Code == 2)
        {
            playerToSpawn = redPlayerPrefabs[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];
        }

        playerToSpawn.GetComponent<Tank>().teamIndex = (int)PhotonNetwork.LocalPlayer.CustomProperties["playerTeam"];
        playerToSpawn.GetComponent<Tank>().respawnPoint = spawnPoints[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerTeam"] - 1];
        PhotonNetwork.Instantiate(playerToSpawn.name, randomPosition, Quaternion.identity);
    }
}
