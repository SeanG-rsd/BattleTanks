using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnPlayers : MonoBehaviour
{

    public GameObject[] bluePlayerPrefabs;
    public GameObject[] redPlayerPrefabs;
    GameObject playerToSpawn;

    public string[] teamNames;

    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;
    public float Y;
    // Start is called before the first frame update
    void Start()
    {
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

        //playerToSpawn.tag = teamNames[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerTeam"]];
        PhotonNetwork.Instantiate(playerToSpawn.name, randomPosition, Quaternion.identity);

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
