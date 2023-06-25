using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnPlayers : MonoBehaviour
{

    public GameObject playerPrefab;

    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;
    public float Y;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 randomPosition = new Vector3(Random.Range(minX, maxX), Y, Random.Range(minZ, maxZ));
        PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
