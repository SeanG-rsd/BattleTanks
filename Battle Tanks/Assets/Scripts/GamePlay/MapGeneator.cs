using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneator : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject wallPrefab;

    [SerializeField] private Transform cellContainer;
    [SerializeField] private Transform wallContainer;

    private Vector2 gridSize;
    [SerializeField] int originalWallScale;

    private GameMode selectedGameMode;

    [SerializeField] private List<GameObject> spawnTowers;
    private List<Vector3> towerPositions;

    public static Action OnMapGenerated = delegate { };
    private void Awake() // in the future make sure that the map is good or not
    {
        towerPositions = new List<Vector3>();

        GameManager.OnGenerateMap += HandleMapGeneration;
    }

    private void OnDestroy()
    {
        GameManager.OnGenerateMap -= HandleMapGeneration;
    }

    private void HandleMapGeneration(Vector2 gridSize, GameMode selectedGameMode)
    {
        this.gridSize = gridSize;
        HandleWalls();
        HandleCells();
        //this.selectedGameMode = selectedGameMode;
        HandleGameMode(selectedGameMode);

        Hashtable mapGenerated = new Hashtable() { { "mapGeneration", 0 } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(mapGenerated);
    }

    private void HandleWalls()
    {
        if (wallPrefab != null)
        {
            bool other = false;
            bool directionX = false;
            bool directionZ = false;

            for (int i = 0; i < 4; ++i)
            {
                GameObject newWall = PhotonNetwork.Instantiate(wallPrefab.name, wallContainer.position, Quaternion.identity);

                Vector3 newWallScale = new Vector3(originalWallScale * gridSize.x, newWall.transform.localScale.y, newWall.transform.localScale.z);
                newWall.transform.localScale = newWallScale;

                

                Vector3 newWallPos = Vector3.zero;

                if (!other)
                {
                    newWallPos = new Vector3(originalWallScale * gridSize.x / 2, 0, 0);

                    other = !other;
                }
                else
                {
                    newWallPos = new Vector3(0, 0, originalWallScale * gridSize.x / 2);

                    other = !other;
                }

                if (newWallPos.x != 0)
                {
                    Vector3 newWallRotation = new Vector3(0, 90, 0);
                    newWall.transform.Rotate(newWallRotation);

                    if (!directionX)
                    {
                        newWallPos.x = -newWallPos.x;
                        directionX = !directionX;
                    }
                }
                else if (newWallPos.z != 0 && !directionZ)
                {
                    newWallPos.z = -newWallPos.z;

                    directionZ = !directionZ;
                }
                newWallPos.y = 1.5f;

                newWall.transform.localPosition = newWallPos;

            }
        }
    }

    private void HandleCells()
    {
        int max = (int)(originalWallScale * gridSize.x / 2);

        int xIndex = (int)UnityEngine.Random.Range(1, gridSize.x);
        int zIndex = (int)UnityEngine.Random.Range(1, gridSize.y);

        for (int z = 1; z <= gridSize.y; ++z)
        {
            for (int x = 1; x <= gridSize.x; ++x)
            {

                GameObject newCell = PhotonNetwork.Instantiate(cellPrefab.name, cellContainer.position, Quaternion.identity);

                Vector3 newCellPos = Vector3.zero;

                newCellPos.y = 1.5f;

                newCellPos.x = -max + (originalWallScale * x);
                newCellPos.z = max - (originalWallScale * z);

                newCell.transform.localPosition = newCellPos;

                if (z == 1 && x == xIndex)
                {
                    Vector3 towerPos = newCellPos;
                    towerPos.x -= originalWallScale / 2;
                    towerPos.z += originalWallScale / 2;
                    towerPos.y = 1.6657f;

                    towerPositions.Add(towerPos);
                }
                else if (z == gridSize.y && x == zIndex)
                {
                    Vector3 towerPos = newCellPos;
                    towerPos.x -= originalWallScale / 2;
                    towerPos.z += originalWallScale / 2;
                    towerPos.y = 1.6657f;

                    towerPositions.Add(towerPos);
                }
            }
        }
    }

    private void HandleGameMode(GameMode gameMode)
    {
        for (int i = 0; i < spawnTowers.Count; i++)
        {
            if (gameMode != null)
            {
                spawnTowers[i].SetActive(gameMode.HasTeams);
            }
            else
            {
                Debug.Log("No selected game mode detected when generating map");
            }
            spawnTowers[i].transform.localPosition = towerPositions[i];
        }
    }

    private void GuaranteePlayability()
    {

    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("mapGeneration"))
        {
            Debug.Log("map has been generated");
            OnMapGenerated?.Invoke();
        }
    }
}