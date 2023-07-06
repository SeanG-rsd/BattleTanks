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

    TeamInfo teamInfo;

    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;
    public float Y;

    PhotonTeam team;
    // Start is called before the first frame update
    void Start()
    {
        
        teamInfo = FindObjectOfType<TeamInfo>();
        Vector3 randomPosition = new Vector3(Random.Range(minX, maxX), Y, Random.Range(minZ, maxZ));
        //GameObject playerToSpawn = playerPrefabs[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];\
        if ((int)PhotonNetwork.LocalPlayer.CustomProperties["playerTeam"] == 0)
        {
            playerToSpawn = bluePlayerPrefabs[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];
            
        }
        else if ((int)PhotonNetwork.LocalPlayer.CustomProperties["playerTeam"] == 1)
        {
            playerToSpawn = redPlayerPrefabs[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];
        }
        playerToSpawn.GetComponent<Tank>().teamIndex = (int)PhotonNetwork.LocalPlayer.CustomProperties["playerTeam"];
        //team.Name = teamInfo.teamNames[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerTeam"]];
        //team.Code = (byte)PhotonNetwork.LocalPlayer.CustomProperties["playerTeam"];

        //PhotonNetwork.LocalPlayer.JoinTeam(team);
        PhotonNetwork.Instantiate(playerToSpawn.name, randomPosition, Quaternion.identity);

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
