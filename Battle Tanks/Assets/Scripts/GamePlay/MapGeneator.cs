using Photon.Pun;
using System;
using UnityEngine;

public class MapGeneator : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject wallPrefab;

    [SerializeField] private Transform cellContainer;
    [SerializeField] private Transform wallContainer;

    private Vector2 gridSize;
    [SerializeField] int originalWallScale;

    [SerializeField] private Transform[] edgeWalls;

    [SerializeField] private GameObject[] spawnTowers;

    public static Action OnMapGenerated = delegate { };
    private void Awake() // in the future make sure that the map is good or not
    {
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
        HandleGameMode(selectedGameMode);

        OnMapGenerated?.Invoke();
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

                newWall.transform.SetParent(wallContainer);

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


                newWall.transform.localPosition = newWallPos;

            }
        }
    }

    private void HandleCells()
    {
        int max = (int)(originalWallScale * gridSize.x / 2);

        for (int z = 1; z <= gridSize.y; ++z)
        {
            for (int x = 1; x <= gridSize.x; ++x)
            {

                GameObject newCell = PhotonNetwork.Instantiate(cellPrefab.name, cellContainer.position, Quaternion.identity);

                Vector3 newCellPos = Vector3.zero;

                newCellPos.x = -max + (originalWallScale * x);
                newCellPos.z = max - (originalWallScale * z);

                newCell.transform.SetParent(cellContainer);

                newCell.transform.localPosition = newCellPos;

            }
        }
    }

    private void HandleGameMode(GameMode gameMode)
    {

    }
}
