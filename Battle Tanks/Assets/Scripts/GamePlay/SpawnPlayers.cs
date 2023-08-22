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

    [SerializeField] private Camera miniMapCam;
    [SerializeField] private TeamInfo teamInfo;

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
        
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("playerAvatar"))
        {
            PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"] = 2;
        }

        if ((int)PhotonNetwork.LocalPlayer.GetPhotonTeam().Code == 1)
        {
            playerToSpawn = bluePlayerPrefabs[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];

        }
        else if ((int)PhotonNetwork.LocalPlayer.GetPhotonTeam().Code == 2 || (int)PhotonNetwork.LocalPlayer.GetPhotonTeam().Code == 3)
        {
            playerToSpawn = redPlayerPrefabs[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];
        }

        playerToSpawn.GetComponent<Tank>().teamIndex = (int)PhotonNetwork.LocalPlayer.CustomProperties["playerTeam"];

        SetMiniMap((int)PhotonNetwork.LocalPlayer.CustomProperties["playerTeam"]);

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            if (spawnPoints[i].teamIndex == (int)PhotonNetwork.LocalPlayer.GetPhotonTeam().Code)
            {
                playerToSpawn.GetComponent<Tank>().respawnPoint = spawnPoints[i];
                break;
            }
        }

        Vector2 pos = spawnPoints[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerTeam"] - 1].GetPoint();

        Vector3 position = new Vector3(pos.x, playerToSpawn.transform.position.y, pos.y);

        PhotonNetwork.LocalPlayer.CustomProperties["aliveState"] = 1;

        PhotonNetwork.Instantiate(playerToSpawn.name, position, Quaternion.identity);

        OnTankSpawned?.Invoke();
    }

    private void SetMiniMap(int team)
    {
        Debug.Log("set mini map");

        miniMapCam.cullingMask = LayerMask.GetMask("UI", teamInfo.teamNames[team - 1], "See", "MiniMap", $"MiniMap{teamInfo.teamNames[team - 1]}");
    }
}
