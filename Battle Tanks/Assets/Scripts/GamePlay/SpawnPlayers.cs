using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

public class SpawnPlayers : MonoBehaviour
{

    public GameObject[] bluePlayerPrefabs;
    public GameObject[] redPlayerPrefabs;
    GameObject playerToSpawn;

    public List<TankRespawnPoint> spawnPoints;

    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;
    public float Y;

    PhotonView view;

    public static Action OnTankSpawned = delegate { };

    private void Awake()
    {
        MapGeneator.OnMapGenerated += HandleSpawnPlayers;
        MapGeneator.OnSoloTowersGen += HandleSoloGen;
    }

    private void OnDestroy()
    {
        MapGeneator.OnMapGenerated -= HandleSpawnPlayers;
        MapGeneator.OnSoloTowersGen -= HandleSoloGen;
    }

    private void HandleSoloGen(List<GameObject> towers)
    {
        Debug.Log("handle solo gen");

        for (int i = 0; i < spawnPoints.Count; ++i)
        {
            spawnPoints[i].gameObject.SetActive(false);
        }

        spawnPoints.Clear();

        for (int i = 0; i < towers.Count; i++)
        {
            spawnPoints.Add(towers[i].GetComponent<TankRespawnPoint>());
        }
    }

    private void HandleSpawnPlayers()
    {
        Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(minX, maxX), Y, UnityEngine.Random.Range(minZ, maxZ));

        if ((int)PhotonNetwork.LocalPlayer.GetPhotonTeam().Code == 1)
        {
            playerToSpawn = bluePlayerPrefabs[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];

        }
        else if ((int)PhotonNetwork.LocalPlayer.GetPhotonTeam().Code == 2 || (int)PhotonNetwork.LocalPlayer.GetPhotonTeam().Code == 3)
        {
            playerToSpawn = redPlayerPrefabs[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];
        }

        playerToSpawn.GetComponent<Tank>().teamIndex = (int)PhotonNetwork.LocalPlayer.CustomProperties["playerTeam"];

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            if (spawnPoints[i].teamIndex == (int)PhotonNetwork.LocalPlayer.GetPhotonTeam().Code)
            {
                playerToSpawn.GetComponent<Tank>().respawnPoint = spawnPoints[i];
                break;
            }
        }

        //playerToSpawn.GetComponent<Tank>().respawnPoint = spawnPoints[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerTeam"] - 1];

        PhotonNetwork.LocalPlayer.CustomProperties["aliveState"] = 1;

        PhotonNetwork.Instantiate(playerToSpawn.name, randomPosition, Quaternion.identity);

        OnTankSpawned?.Invoke();
    }
}
