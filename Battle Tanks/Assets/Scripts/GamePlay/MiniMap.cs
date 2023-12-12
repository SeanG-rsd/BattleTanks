using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    [SerializeField] private GameObject wallPrefab;
    private float edgeScale;
    [SerializeField] private float originalWallScale;

    [SerializeField] private Transform miniMapContainer;

    [SerializeField] private float updateInterval;
    private float timeTilNextUpdate;

    [SerializeField] private TeamInfo teamInfo;

    private Dictionary<GameObject, GameObject> objectsToUpdate;

    private Vector2 gridSize;

    private int teamIndex;

    [SerializeField] private GameObject tankIconPrefab;

    private void Start()
    {
        Initialize(new Vector2(10, 10), 1);
        edgeScale = 10 * originalWallScale;
        objectsToUpdate = new Dictionary<GameObject, GameObject>();
        HandleWalls();
        MapGeneator.OnMiniMapWalls += HandleCells;
        MapGeneator.OnMiniMapIcon += HandleGameModeObjects;
    }

    private void OnDestroy()
    {
        MapGeneator.OnMiniMapWalls -= HandleCells;
        MapGeneator.OnMiniMapIcon -= HandleGameModeObjects;
    }

    private void Initialize(Vector2 gridS, int teamIndex)
    {
        gridSize = gridS;
        this.teamIndex = teamIndex;

        Tank[] allTanks = FindObjectsOfType<Tank>();
        foreach (Tank tank in allTanks)
        {
            if (tank.teamIndex == teamIndex)
            {
                GameObject tankIcon = Instantiate(tankIconPrefab, miniMapContainer.position, Quaternion.identity);
                objectsToUpdate.Add(tank.gameObject, tankIcon);
            }
        }
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
                for (int count = 0; count < gridSize.x; ++count)
                {
                    GameObject newWall = Instantiate(wallPrefab, miniMapContainer.position, Quaternion.identity);

                    //Vector3 newWallScale = new Vector3(originalWallScale * gridSize.x, newWall.transform.localScale.y, newWall.transform.localScale.z);
                    //newWall.transform.localScale = newWallScale;

                    Vector3 newWallPos;

                    if (!other)
                    {
                        newWallPos = new Vector3(0, originalWallScale * count, 0);



                        if (!directionX)
                        {
                            newWallPos.x = gridSize.x * originalWallScale;
                        }

                    }
                    else
                    {
                        Vector3 newWallRotation = new Vector3(0, 0, 90);
                        newWall.transform.Rotate(newWallRotation);

                        newWallPos = new Vector3(originalWallScale * (count + 1) + 15, 0, 0);
                        if (!directionZ)
                        {
                            newWallPos.y = gridSize.y * originalWallScale;
                        }

                    }
                    newWall.transform.localScale = new Vector3(newWall.transform.localScale.x * miniMapContainer.localScale.x, newWall.transform.localScale.x * miniMapContainer.localScale.x, newWall.transform.localScale.x * miniMapContainer.localScale.x);
                    newWall.transform.SetParent(miniMapContainer);

                    newWall.transform.localPosition = newWallPos;
                }

                if (!other)
                {
                    directionX = !directionX;
                }
                else
                {
                    directionZ = !directionZ;
                }

                other = !other;
            }
        }
    }

    private void HandleCells(List<MapGeneator.WallInfo> allWalls)
    {
        Debug.Log("handle cells");
        List<MapGeneator.WallInfo> cells = allWalls;

        foreach (MapGeneator.WallInfo wallInfo in cells)
        {
            Debug.Log(wallInfo.ToString());
            if (IsWithinBounds(wallInfo.position))
            {
                GameObject newWall = Instantiate(wallPrefab, miniMapContainer.position, Quaternion.identity);
                Vector3 newWallPos = new Vector3(originalWallScale * (wallInfo.position.x + 1), originalWallScale * (9 - wallInfo.position.y), 0);

                if (wallInfo.orientation == WallType.WallOrientation.Horizontal)
                {
                    newWallPos.x += 15;
                    Vector3 newWallRotation = new Vector3(0, 0, 90);
                    newWall.transform.Rotate(newWallRotation);
                }
                newWall.transform.localScale = new Vector3(newWall.transform.localScale.x * miniMapContainer.localScale.x, newWall.transform.localScale.x * miniMapContainer.localScale.x, newWall.transform.localScale.x * miniMapContainer.localScale.x);
                newWall.transform.SetParent(miniMapContainer);
               
                newWall.transform.localPosition = newWallPos;
            }
        }
    }

    private void HandleGameModeObjects(GameObject iconPrefab, Vector2 position, int scale, GameObject objectToFollow, bool isZone)
    {
        int column = (int)((position.x - 1.5) / 3) + 6;
        int row = (int)((position.y - 1.5) / 3) + 6;
        Debug.Log(row + " " + column);
        GameObject icon = Instantiate(iconPrefab, miniMapContainer.position, Quaternion.identity);

        icon.transform.SetParent(miniMapContainer);
        if (!isZone)
        {
            Vector3 iconPosition = new Vector3(originalWallScale * (column) - 40, originalWallScale * (row - 1) + 32, 0);
            icon.transform.localPosition = iconPosition;
        }
        else
        {
            Vector3 iconPosition = new Vector3(originalWallScale * (column - 2), originalWallScale * (row - 2), 0);
            icon.transform.localPosition = iconPosition;
        }
        icon.transform.localScale = new Vector3(scale, scale, scale);
        objectsToUpdate.Add(objectToFollow, icon);
    }

    private bool IsWithinBounds(Vector2 pos)
    {
        return pos.x >= 0 && pos.x < gridSize.x && pos.y >= 0 && pos.y < gridSize.y;
    }
}

