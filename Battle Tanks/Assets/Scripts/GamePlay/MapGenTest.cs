using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.ExceptionServices;
using Unity.VisualScripting;
using UnityEngine;

public class MapGenTest : MonoBehaviour
{

    struct WallInfo
    {
        public Vector2 position;
        public WallType.WallOrientation orientation;
        public bool isTouchingBorder;
    }

    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject wallPrefab;

    [SerializeField] private Transform cellContainer;
    [SerializeField] private Transform wallContainer;

    private Vector2 gridSize;
    [SerializeField] int originalWallScale;

    private GameMode selectedGameMode;

    [SerializeField] private List<GameObject> spawnTowers;
    private List<Vector3> towerPositions;

    [SerializeField] private GameMode[] availableGameModes;
    [SerializeField] private Vector2[] possibleMapSizes;

    public static Action OnTestMapGen = delegate { };

    private Dictionary<WallInfo, GameObject> walls;

    private List<GameObject> cellObjects;

    private void Start()
    {
        towerPositions = new List<Vector3>();

        walls = new Dictionary<WallInfo, GameObject>();

        cellObjects = new List<GameObject>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DestroyCurrentMap();
            towerPositions.Clear();
            HandleMapGeneration(possibleMapSizes[0], availableGameModes[0]);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            GuaranteePlayability();
        }
    }

    private void DestroyCurrentMap()
    {
        for (int i = 0; i < cellContainer.childCount; ++i)
        {
            Destroy(cellContainer.GetChild(i).gameObject);
        }

        for (int i = 0; i < wallContainer.childCount; ++i)
        {
            Destroy(wallContainer.GetChild(i).gameObject);
        }
    }

    private void HandleMapGeneration(Vector2 gridSize, GameMode selectedGameMode)
    {
        this.gridSize = gridSize;
        HandleWalls();
        HandleCells();
        //this.selectedGameMode = selectedGameMode;
        HandleGameMode(selectedGameMode);

        OnTestMapGen?.Invoke();
        //GuaranteePlayability();
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
                GameObject newWall = Instantiate(wallPrefab, wallContainer.position, Quaternion.identity);

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

                newWall.transform.SetParent(wallContainer);
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

                GameObject newCell = Instantiate(cellPrefab, cellContainer.position, Quaternion.identity);

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

                newCell.transform.SetParent(cellContainer);

                for (int i = 0; i < newCell.transform.childCount; i++)
                {
                    newCell.transform.GetChild(i).gameObject.GetComponent<MapWall>().position = new Vector2(x, z);

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

    private List<WallInfo> GetPossibleInfo(WallInfo original)
    {
        List<WallInfo> possibleWalls = new List<WallInfo>();

        

        if (original.orientation == WallType.WallOrientation.Horizontal)
        {

            WallInfo left = new WallInfo { position = new Vector2(original.position.x - 1, original.position.y), orientation = original.orientation };
            left.isTouchingBorder = GetWallBool(left);
            WallInfo leftUp = new WallInfo { position = new Vector2(original.position.x - 1, original.position.y), orientation = WallType.WallOrientation.Vertical };
            leftUp.isTouchingBorder = GetWallBool(leftUp);
            WallInfo leftDown = new WallInfo { position = new Vector2(original.position.x - 1, original.position.y + 1), orientation = WallType.WallOrientation.Vertical };
            leftDown.isTouchingBorder = GetWallBool(leftDown);
            WallInfo right = new WallInfo { position = new Vector2(original.position.x + 1, original.position.y), orientation = original.orientation };
            right.isTouchingBorder = GetWallBool(right);
            WallInfo rightUp = new WallInfo { position = new Vector2(original.position.x, original.position.y), orientation = WallType.WallOrientation.Vertical };
            rightUp.isTouchingBorder = GetWallBool(rightUp);
            WallInfo rightDown = new WallInfo { position = new Vector2(original.position.x, original.position.y + 1), orientation = WallType.WallOrientation.Vertical };
            rightDown.isTouchingBorder = GetWallBool(rightDown);

            possibleWalls.Add(left);
            possibleWalls.Add(leftUp);
            possibleWalls.Add(leftDown);
            possibleWalls.Add(right);
            possibleWalls.Add(rightUp);
            possibleWalls.Add(rightDown);
        }
        else if (original.orientation == WallType.WallOrientation.Vertical)
        {
            WallInfo up = new WallInfo { position = new Vector2(original.position.x, original.position.y - 1), orientation = original.orientation };
            up.isTouchingBorder = GetWallBool(up);
            WallInfo upLeft = new WallInfo { position = new Vector2(original.position.x, original.position.y - 1), orientation = WallType.WallOrientation.Horizontal };
            upLeft.isTouchingBorder = GetWallBool(upLeft);
            WallInfo upRight = new WallInfo { position = new Vector2(original.position.x + 1, original.position.y - 1), orientation = WallType.WallOrientation.Horizontal };
            upRight.isTouchingBorder = GetWallBool(upRight);
            WallInfo down = new WallInfo { position = new Vector2(original.position.x, original.position.y + 1), orientation = original.orientation };
            down.isTouchingBorder = GetWallBool(down);
            WallInfo downLeft = new WallInfo { position = new Vector2(original.position.x, original.position.y), orientation = WallType.WallOrientation.Horizontal };
            downLeft.isTouchingBorder = GetWallBool(downLeft);
            WallInfo downRight = new WallInfo { position = new Vector2(original.position.x + 1, original.position.y), orientation = WallType.WallOrientation.Horizontal };
            downRight.isTouchingBorder = GetWallBool(downRight);

            possibleWalls.Add(up);
            possibleWalls.Add(upLeft);
            possibleWalls.Add(upRight);
            possibleWalls.Add(down);
            possibleWalls.Add(downLeft);
            possibleWalls.Add(downRight);
        }

        return possibleWalls;
    }

    private bool GetWallBool(WallInfo wallInfo)
    {
        WallInfo newWallInfo = new() { isTouchingBorder = false };

        if (wallInfo.orientation == WallType.WallOrientation.Horizontal)
        {
            if (wallInfo.position.x == 1 || wallInfo.position.x == gridSize.x)
            {
                newWallInfo.isTouchingBorder = true;
            }
        }
        else if (wallInfo.orientation == WallType.WallOrientation.Vertical)
        {
            if (wallInfo.position.y == 1 || wallInfo.position.y == gridSize.y)
            {
                newWallInfo.isTouchingBorder = true;
            }
        }

        return newWallInfo.isTouchingBorder;
    }

    private void SetDictionary()
    {
        walls.Clear();

        for (int i = 0; i < cellContainer.childCount; ++i)
        {
            for (int ii = 0; ii < cellContainer.GetChild(i).childCount; ++ii)
            {
                WallInfo newWallInfo = GetInfo(cellContainer.GetChild(i).GetChild(ii).gameObject);

                walls.Add(newWallInfo, cellContainer.GetChild(i).GetChild(ii).gameObject);
            }
        }

        Debug.Log(walls.Count);
    }

    private WallInfo GetInfo(GameObject wall)
    {
        WallInfo newWallInfo = new();
        newWallInfo.position = wall.GetComponent<MapWall>().position;
        newWallInfo.orientation = wall.GetComponent<MapWall>().type;

        if (wall.GetComponent<MapWall>().type == WallType.WallOrientation.Horizontal)
        {
            if (wall.GetComponent<MapWall>().position.x == 1 || wall.GetComponent<MapWall>().position.x == gridSize.x)
            {
                newWallInfo.isTouchingBorder = true;
                wall.GetComponent<MapWall>().isTouchingBorder = true;
            }
        }
        else if (wall.GetComponent<MapWall>().type == WallType.WallOrientation.Vertical)
        {
            if (wall.GetComponent<MapWall>().position.y == 1 || wall.GetComponent<MapWall>().position.y == gridSize.y)
            {
                newWallInfo.isTouchingBorder = true;
                wall.GetComponent<MapWall>().isTouchingBorder = true;
            }
        }

        return newWallInfo;
    }

    private void CheckForBoxes(List<GameObject> segment)
    {
        Debug.LogWarning("check for boxes");
        Debug.Log(segment.Count);
        int borderCount = 0;

        foreach (GameObject obj in segment)
        {
            if (obj.GetComponent<MapWall>().isTouchingBorder)
            {
                borderCount++;
            }
            Debug.Log($"{obj.GetComponent<MapWall>().position} is {obj.GetComponent<MapWall>().isTouchingBorder}");
        }

        if (borderCount >= 2)
        {
            Debug.LogWarning("there is a box");
            int index = UnityEngine.Random.Range(0, segment.Count);
            Destroy(segment[index]);
        }
    }

    private void GuaranteePlayability()
    {
        List<List<WallInfo>> listsOfInfo = new List<List<WallInfo>>();
        List<List<GameObject>> allSegements = new List<List<GameObject>>();

        List<GameObject> checkedWalls = new List<GameObject>();
        SetDictionary();

        for (int i = 0; i < cellContainer.childCount; i++)
        { 
            for (int wallCount = 0; wallCount < cellContainer.GetChild(i).childCount; wallCount++)
            {
                WallInfo newWallInfo = GetInfo(cellContainer.GetChild(i).GetChild(wallCount).gameObject);
                

                GameObject value;

                listsOfInfo.Add(GetPossibleInfo(newWallInfo));

                List<GameObject> segment = new()
                    {
                        cellContainer.GetChild(i).GetChild(wallCount).gameObject
                    };

                while (listsOfInfo.Count > 0)
                {
                    for (int ii = 0; ii < listsOfInfo[0].Count; ii++)
                    {
                        if (walls.TryGetValue(listsOfInfo[0][ii], out value))
                        {
                            if (!segment.Contains(value))
                            {
                                newWallInfo = GetInfo(value);
                                segment.Add(value);

                                listsOfInfo.Add(GetPossibleInfo(newWallInfo));
                            }
                        }
                    }

                    listsOfInfo.RemoveAt(0);
                }

                if (segment.Count > 1)
                {
                    bool hasSeenBefore = true;

                    for (int h = 0; h < segment.Count; h++)
                    {
                        if (checkedWalls.Contains(segment[h]))
                        {
                            hasSeenBefore = true;
                            break;
                        }

                        hasSeenBefore = false;
                    }
                    if (!hasSeenBefore)
                    {
                        allSegements.Add(segment);
                    }
                }

                foreach (GameObject list in segment)
                {
                    checkedWalls.Add(list);
                }
            }
        }

        foreach (List<GameObject> seg in allSegements)
        {
            CheckForBoxes(seg);
        }
    }
}

