using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Security;
using UnityEditor;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    [SerializeField] private GameObject wallPrefab;
    private float edgeScale;
    [SerializeField] private float originalWallScale;

    [SerializeField] public Transform miniMapContainer;

    [SerializeField] private float updateInterval = 0.5f;
    private float timeTilNextUpdate;

    [SerializeField] private TeamInfo teamInfo;

    private Dictionary<GameObject, GameObject> objectsToUpdate;

    private Vector2 gridSize;

    private int teamIndex;

    private bool spawnedTankIcons = false;

    [SerializeField] private GameObject[] tankIconPrefab;

    private GameObject localTankToFollow;

    private List<GameObject> indicators;
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private Transform indicatorContainer;

    private float indicatorVisibilityRange;
    private float miniMapContainerScale;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Initialize(new Vector2(10, 10), 1);
            edgeScale = 10 * originalWallScale;
            objectsToUpdate = new Dictionary<GameObject, GameObject>();
            HandleWalls();
        }
        indicatorVisibilityRange = miniMapContainer.parent.gameObject.GetComponent<RectTransform>().rect.width / 2;
        miniMapContainerScale = miniMapContainer.localScale.x;
        indicators = new List<GameObject>();
        MapGeneator.OnMiniMapWalls += HandleCells;
        MapGeneator.OnMiniMapIcon += HandleGameModeObjects;
        Icon.OnTankIconMade += HandleTankIconMade;
        Icon.OnMakeLocalIndicator += HandleIndicatorMade;
        
    }

    private void OnDestroy()
    {
        MapGeneator.OnMiniMapWalls -= HandleCells;
        MapGeneator.OnMiniMapIcon -= HandleGameModeObjects;
        Icon.OnTankIconMade -= HandleTankIconMade;
        Icon.OnMakeLocalIndicator -= HandleIndicatorMade;
    }

    private void Update()
    {
        if (localTankToFollow != null)
        {
            //Debug.Log(localTankToFollow.transform.localPosition);
            miniMapContainer.localPosition = new Vector2(-localTankToFollow.transform.localPosition.x * miniMapContainer.transform.localScale.x, -localTankToFollow.transform.localPosition.y * miniMapContainer.transform.localScale.y);
            UpdateIndicators();
        }

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            while (FindObjectsOfType<Tank>().Length == PhotonNetwork.CurrentRoom.PlayerCount && !spawnedTankIcons)
            {
                HandleTanks();
                spawnedTankIcons = true;
            }

            if (timeTilNextUpdate >= 0)
            {
                timeTilNextUpdate -= Time.deltaTime;
                if (timeTilNextUpdate <= 0)
                {
                    //Debug.Log("updateObjects");
                    timeTilNextUpdate = updateInterval;
                    UpdateObjects();
                }
            }
        }

        if (FindObjectsOfType<Tank>().Length == PhotonNetwork.CurrentRoom.PlayerCount && !spawnedTankIcons)
        {
            spawnedTankIcons = true;
        }
    }

    private void HandleTankIconMade(GameObject obj)
    {
        localTankToFollow = obj;
    }

    private void HandleIndicatorMade(Sprite s, Color c, GameObject icon)
    {
        GameObject indicator = Instantiate(indicatorPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        if (indicator != null)
        {
            Debug.Log(icon.name);
            indicator.GetComponent<MiniMapIndicator>().SetIcon(s, c, icon);
            //indicator.transform.SetParent(indicatorContainer);

            indicators.Add(indicator);
        }
    }

    private void UpdateIndicators()
    {
        //Debug.Log(indicatorVisibilityRange);
        foreach (GameObject indicator in indicators)
        {
            if (indicator != null)
            {
                GameObject icon = indicator.GetComponent<MiniMapIndicator>().icon;
   
                if (Vector3.Distance(icon.transform.localPosition, localTankToFollow.transform.localPosition) < indicatorVisibilityRange / miniMapContainerScale)
                {
                    indicator.SetActive(false);
                }
                else
                {
                    indicator.SetActive(true);

                    float changeInX = (icon.transform.localPosition.x - localTankToFollow.transform.localPosition.x) / miniMapContainerScale;
                    float changeInY = (icon.transform.localPosition.y - localTankToFollow.transform.localPosition.y) / miniMapContainerScale;
                    //Debug.Log($"x : {changeInX}, y : {changeInY}");

                    if (changeInX == 0)
                    {
                        indicator.transform.localPosition = new Vector3(0, indicatorVisibilityRange * (changeInY > 0 ? 1 : -1), 0);
                        //Debug.Log("to the right");
                    }
                    else if (changeInY == 0)
                    {
                        indicator.transform.localPosition = new Vector3(indicatorVisibilityRange * (changeInX > 0 ? 1 : -1), 0, 0);
                        //Debug.Log("to the left");
                    }
                    else
                    {
                        float slope = changeInY / changeInX;
                        float x = Mathf.Sqrt(indicatorVisibilityRange * indicatorVisibilityRange / (1 + (slope * slope)));
                        float y = slope * x * (slope > 0 ? 1 : -1);
                       // Debug.Log($"x : {x}, y : {y}");

                        indicator.transform.localPosition = new Vector3(x * (changeInX > 0 ? 1 : -1), y * (changeInY > 0 ? 1 : -1), 0);
                    }
                }
            }
        }
    }

    private void UpdateObjects()
    {
        MapCell[] mapCells = FindObjectsOfType<MapCell>();
        foreach (GameObject obj in objectsToUpdate.Keys)
        {
            if (obj != null)
            {
                MapCell closest = null;
                foreach (MapCell cell in mapCells)
                {
                    if (closest == null)
                    {
                        closest = cell;
                    }
                    else if (Vector3.Distance(cell.center.transform.position, obj.transform.position) < Vector3.Distance(closest.center.transform.position, obj.transform.position))
                    {
                        closest = cell;
                    }
                }
                UpdateIcon(objectsToUpdate[obj], closest.position);
            }
            else
            {
                objectsToUpdate[obj].GetComponent<Icon>().Destroy();
                objectsToUpdate.Remove(obj);
            }
        }
    }

    private void UpdateIcon(GameObject icon, Vector2 position)
    {
        icon.transform.localPosition = new Vector3(position.x * originalWallScale + 40, (9 - position.y) * originalWallScale + 32, 0);
    }

    private void Initialize(Vector2 gridS, int teamIndex)
    {
        gridSize = gridS;
        this.teamIndex = teamIndex;
    }

    private void HandleTanks()
    {
        Tank[] allTanks = FindObjectsOfType<Tank>();
        foreach (Tank tank in allTanks)
        {
            GameObject tankIcon = PhotonNetwork.Instantiate(tankIconPrefab[tank.teamIndex - 1].name, miniMapContainer.position, Quaternion.identity, 0, new object[] { tank.GetComponent<PhotonView>().Owner, tankIconPrefab[tank.teamIndex - 1].transform.localScale.x}); // who is the owner of the tank its following, is it an indicator on the map
            //Debug.Log("handleTanks");
            objectsToUpdate.Add(tank.gameObject, tankIcon);
            //tankIcon.transform.SetParent(miniMapContainer);
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
                    GameObject newWall = PhotonNetwork.Instantiate(wallPrefab.name, miniMapContainer.position, Quaternion.identity, 0, new object[] { PhotonNetwork.LocalPlayer, wallPrefab.transform.localScale.x * miniMapContainer.localScale.x });

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
                    //newWall.transform.SetParent(miniMapContainer);

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
        //Debug.Log("handle cells");
        List<MapGeneator.WallInfo> cells = allWalls;

        foreach (MapGeneator.WallInfo wallInfo in cells)
        {
            Debug.Log(wallInfo.ToString());
            if (IsWithinBounds(wallInfo.position))
            {
                GameObject newWall = PhotonNetwork.Instantiate(wallPrefab.name, miniMapContainer.position, Quaternion.identity, 0, new object[] { PhotonNetwork.LocalPlayer, wallPrefab.transform.localScale.x * miniMapContainer.localScale.x });
                Vector3 newWallPos = new Vector3(originalWallScale * (wallInfo.position.x + 1), originalWallScale * (9 - wallInfo.position.y), 0);

                if (wallInfo.orientation == WallType.WallOrientation.Horizontal)
                {
                    newWallPos.x += 15;
                    Vector3 newWallRotation = new Vector3(0, 0, 90);
                    newWall.transform.Rotate(newWallRotation);
                }
                newWall.transform.localScale = new Vector3(newWall.transform.localScale.x * miniMapContainer.localScale.x, newWall.transform.localScale.x * miniMapContainer.localScale.x, newWall.transform.localScale.x * miniMapContainer.localScale.x);
                //newWall.transform.SetParent(miniMapContainer);
               
                newWall.transform.localPosition = newWallPos;
            }
        }
    }

    private void HandleGameModeObjects(GameObject iconPrefab, Vector2 position, int scale, GameObject objectToFollow, bool isZone)
    {
        int column = (int)((position.x - 1.5) / 3) + 6;
        int row = (int)((position.y - 1.5) / 3) + 6;
        Debug.Log(row + " " + column);
        GameObject icon = PhotonNetwork.Instantiate(iconPrefab.name, miniMapContainer.position, Quaternion.identity, 0, new object[] { PhotonNetwork.LocalPlayer, scale});

        //icon.transform.SetParent(miniMapContainer);
        if (!isZone)
        {
            Vector3 iconPosition = new Vector3(originalWallScale * (column) - 40, originalWallScale * (row - 1) + 32, 0);
            icon.transform.localPosition = iconPosition;
            objectsToUpdate.Add(objectToFollow, icon);
        }
        else
        {
            Vector3 iconPosition = new Vector3(originalWallScale * (column - 2), originalWallScale * (row - 2), 0);
            icon.transform.localPosition = iconPosition;
        }
        icon.transform.localScale = new Vector3(scale, scale, scale);
    }

    private bool IsWithinBounds(Vector2 pos)
    {
        return pos.x >= 0 && pos.x < gridSize.x && pos.y >= 0 && pos.y < gridSize.y;
    }
}

