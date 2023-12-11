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

    private Vector2 gridSize;

    private void Start()
    {
        Initialize(new Vector2(10, 10));
        edgeScale = 10 * originalWallScale;
        HandleWalls();
        MapGenTest.OnMiniMapWalls += HandleCells;
    }

    private void OnDestroy()
    {
        MapGenTest.OnMiniMapWalls -= HandleCells;
    }

    private void Initialize(Vector2 gridS)
    {
        gridSize = gridS;
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

    private void HandleCells(List<MapGenTest.WallInfo> allWalls)
    {
        Debug.Log("handle cells");
        List<MapGenTest.WallInfo> cells = allWalls;

        foreach (MapGenTest.WallInfo wallInfo in cells)
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

    private bool IsWithinBounds(Vector2 pos)
    {
        return pos.x >= 0 && pos.x < gridSize.x && pos.y >= 0 && pos.y < gridSize.y;
    }
}

