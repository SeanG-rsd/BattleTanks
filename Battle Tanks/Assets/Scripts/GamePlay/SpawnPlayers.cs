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
        
        if ((int)PhotonNetwork.LocalPlayer.GetPhotonTeam().Code == 1)
        {
            playerToSpawn = bluePlayerPrefabs[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];
            
        }
        else if ((int)PhotonNetwork.LocalPlayer.GetPhotonTeam().Code == 2)
        {
            playerToSpawn = redPlayerPrefabs[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];
        }
        
        PhotonNetwork.Instantiate(playerToSpawn.name, randomPosition, Quaternion.identity);

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
