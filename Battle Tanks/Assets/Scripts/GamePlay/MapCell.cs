using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCell : MonoBehaviour
{
    public GameObject[] thisWalls;
    private void Awake()
    {
        MapGeneator.OnMapGenerated += HandleMapGenerated;
        MapGenTest.OnTestMapGen += HandleTestMap;
    }

    private void OnDestroy()
    {
        MapGeneator.OnMapGenerated -= HandleMapGenerated;
        MapGenTest.OnTestMapGen -= HandleTestMap;
    }

    private void HandleMapGenerated()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int second = Random.Range(0, 100);

            if (second > 75)
            {
                thisWalls[0].GetComponent<MapWall>().Destroy();
                thisWalls[1].GetComponent<MapWall>().Destroy();

                //PhotonNetwork.Destroy(thisWalls[0]);
                //PhotonNetwork.Destroy(thisWalls[1]);

                return;
            }

            int choice = Random.Range(0, thisWalls.Length);

            //PhotonNetwork.Destroy(thisWalls[choice]);
            thisWalls[choice].GetComponent<MapWall>().Destroy();
        }
    }

    public void HandleTestMap()
    {
        int second = Random.Range(0, 100);

        if (second > 75)
        {
            Destroy(thisWalls[0]);
            Destroy(thisWalls[1]);
            return;
        }

        int choice = Random.Range(0, thisWalls.Length);

        Destroy(thisWalls[choice]);
    }
}
