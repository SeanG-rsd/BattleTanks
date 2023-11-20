using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using Photon.Realtime;

public class SpawnPlayers : MonoBehaviour
{

    public List<GameObject> bluePlayerPrefabs;
    public List<GameObject> redPlayerPrefabs;
    public List<GameObject> greenPlayerPrefabs;
    public List<GameObject> yellowPlayerPrefabs;
    public List<GameObject> orangePlayerPrefabs;
    public List<GameObject> purplePlayerPrefabs;
    public List<GameObject> pinkPlayerPrefabs;
    public List<GameObject> lightBluePlayerPrefabs;

    private List<List<GameObject>> allTankPrefabs;

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
        PhotonTeam[] teams = PhotonTeamsManager.Instance.GetAvailableTeams();
        foreach(PhotonTeam team in teams)
        {
            Debug.Log(team.ToString());
        }
        Debug.Log(PhotonNetwork.InRoom);
        //Debug.Log($"{PhotonNetwork.LocalPlayer.NickName} is on team {PhotonNetwork.LocalPlayer.GetPhotonTeam()}");

        allTankPrefabs = new List<List<GameObject>>
        {
            bluePlayerPrefabs,
            redPlayerPrefabs,
            greenPlayerPrefabs,
            yellowPlayerPrefabs,
            orangePlayerPrefabs,
            purplePlayerPrefabs,
            pinkPlayerPrefabs,
            lightBluePlayerPrefabs
        };


        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("playerAvatar"))
        {
            PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"] = 1;
        }

        playerToSpawn = allTankPrefabs[(int)PhotonNetwork.LocalPlayer.GetPhotonTeam().Code - 1][(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];
        //playerToSpawn = allTankPrefabs[0][0];

        SetMiniMap((int)PhotonNetwork.LocalPlayer.CustomProperties["playerTeam"]);

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            if (spawnPoints[i].teamIndex == (int)PhotonNetwork.LocalPlayer.GetPhotonTeam().Code)
            {
                playerToSpawn.GetComponent<Tank>().respawnPoint = spawnPoints[i];
                break;
            }
        }

        Vector2 pos = new Vector2();

        foreach (TankRespawnPoint tankRespawnPoint in spawnPoints)
        {
            if (tankRespawnPoint.teamIndex == (int)PhotonNetwork.LocalPlayer.GetPhotonTeam().Code - 1)
            {
                pos = tankRespawnPoint.GetPoint();
            }
        }
         

        PhotonNetwork.LocalPlayer.CustomProperties["aliveState"] = 1;

        PhotonNetwork.Instantiate(playerToSpawn.name, playerToSpawn.transform.position, Quaternion.identity);

        OnTankSpawned?.Invoke();
    }

    private void SetMiniMap(int team)
    {
        Debug.Log("set mini map");

        miniMapCam.cullingMask = LayerMask.GetMask("UI", teamInfo.teamNames[team - 1], "See", "MiniMap", $"MiniMap{teamInfo.teamNames[team - 1]}");
    }
}
