using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class MapGeneator : MonoBehaviourPunCallbacks
{
    struct WallInfo
    {
        public Vector2 position;
        public WallType.WallOrientation orientation;
        public bool isTouchingBorder;
    }

    private enum Direction
    {
        left,
        right,
        up,
        down
    }

    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject wallPrefab;

    [SerializeField] private Transform cellContainer;
    [SerializeField] private Transform wallContainer;

    private Vector2 gridSize;
    [SerializeField] int originalWallScale;

    [SerializeField] private List<GameObject> spawnTowers;
    private List<Vector3> towerPositions;

    [SerializeField] private List<GameObject> teamFlags;
    private List<Vector3> flagPositions;

    public GameMode selectedGameMode;

    [SerializeField] private List<GameObject> safeZoneObjects;

    [SerializeField] private GameObject controlZoneObject;
    [SerializeField] private int controlZoneScale;

    [SerializeField] private Vector2[] possibleMapSizes;
    [SerializeField] private GameMode[] availableGameModes;

    [SerializeField] private List<GameObject> soloSpawnTowerPrefabs;
    [SerializeField] private List<Vector3> soloSpawnTowerPositions;
    private List<int> soloTowerOrder = new List<int>() { 0, 2, 5, 7, 1, 6, 3, 4 };

    public static Action OnMapGenerated = delegate { };
    public static Action<List<GameObject>> OnSoloTowersGen = delegate { };

    private List<WallInfo> walls;

    private List<List<WallInfo>> cells;

    private List<WallType.WallOrientation> wallOrientationsForCell = new List<WallType.WallOrientation>() { WallType.WallOrientation.Horizontal, WallType.WallOrientation.Vertical };

    private List<Direction> directions = new List<Direction>() { Direction.left, Direction.right, Direction.up, Direction.down };

    private int blueSpawnIndex;
    private int redSpawnIndex;

    private int blueFlagIndex;
    private int redFlagIndex;
    private void Awake() // in the future make sure that the map is good or not
    {
        towerPositions = new List<Vector3>();
        walls = new List<WallInfo>();
        cells = new List<List<WallInfo>>();
        flagPositions = new List<Vector3>();

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


        this.selectedGameMode = selectedGameMode;
        

        DeleteWalls();
        GuaranteePlayability();
        DoPhysicalMap();

        HandleGameMode();

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

        blueSpawnIndex = (int)UnityEngine.Random.Range(1, gridSize.x);
        redSpawnIndex = (int)UnityEngine.Random.Range(1, gridSize.x);

        blueFlagIndex = (int)UnityEngine.Random.Range(1, gridSize.x);
        redFlagIndex = (int)UnityEngine.Random.Range(1, gridSize.x);

        while (blueFlagIndex == blueSpawnIndex)
        {
            blueFlagIndex = (int)UnityEngine.Random.Range(1, gridSize.x);
        }

        while (redSpawnIndex == redFlagIndex)
        {
            redFlagIndex = (int)UnityEngine.Random.Range(1, gridSize.x);
        }

        for (int z = 1; z <= gridSize.y; ++z)
        {
            for (int x = 1; x <= gridSize.x; ++x)
            {

                GameObject newCell = PhotonNetwork.Instantiate(cellPrefab.name, cellContainer.position, Quaternion.identity);
                List<WallInfo> cellInfo = new List<WallInfo>();

                Vector3 newCellPos = Vector3.zero;

                newCellPos.y = 1.5f;

                newCellPos.x = -max + (originalWallScale * x);
                newCellPos.z = max - (originalWallScale * z);

                newCell.transform.position = newCellPos;

                SetGameModeObjects(newCellPos, z, x);

                newCell.transform.SetParent(cellContainer);

                for (int i = 0; i < newCell.transform.childCount; i++)
                {
                    newCell.transform.GetChild(i).gameObject.GetComponent<MapWall>().position = new Vector2(x - 1, z - 1);
                    WallInfo newWall = new WallInfo() { position = new Vector2(x - 1, z - 1), orientation = wallOrientationsForCell[i] };
                    newWall.isTouchingBorder = GetWallBool(newWall);
                    Debug.Log($"Added {newWall.position} with {newWall.orientation} and {newWall.isTouchingBorder}");

                    cellInfo.Add(newWall);
                }

                cells.Add(cellInfo);
            }
        }
    }

    private void SetGameModeObjects(Vector3 newCellPos, int row, int column)
    {
        Vector3 towerPos = newCellPos;
        towerPos.x -= originalWallScale / 2 + 0.5f;
        towerPos.z += originalWallScale / 2 + 0.5f;
        towerPos.y = 1.6657f;

        Vector3 flagPos = newCellPos;
        flagPos.x -= originalWallScale / 2 + 0.5f;
        flagPos.z += originalWallScale / 2 + 0.5f;
        flagPos.y = 0.875f;

        if (row == 1 && column == blueSpawnIndex)
        {
            towerPositions.Add(towerPos);
        }
        else if (row == 1 && column == blueFlagIndex)
        {
            flagPositions.Add(flagPos);
        }
        else if (row == gridSize.y && column == redSpawnIndex)
        {
            towerPositions.Add(towerPos);
        }
        else if (row == gridSize.y && column == redFlagIndex)
        {
            flagPositions.Add(flagPos);
        }

        if (gridSize == possibleMapSizes[2] || gridSize == possibleMapSizes[1])
        {
            Vector3 newScale = controlZoneObject.transform.localScale;
            newScale.x = 2 * controlZoneScale;
            newScale.z = 2 * controlZoneScale;

            controlZoneObject.transform.localScale = newScale;
        }
        else if (controlZoneObject.transform.localScale.x == (2 * controlZoneScale))
        {
            Vector3 newScale = controlZoneObject.transform.localScale;
            newScale.x = controlZoneScale;
            newScale.z = controlZoneScale;

            controlZoneObject.transform.localScale = newScale;
        }

        if (row == 1 && column == 1)
        {
            soloSpawnTowerPositions.Add(towerPos);
        }
        else if (row == 1 && column == (int)(gridSize.x / 2))
        {
            soloSpawnTowerPositions.Add(towerPos);
        }
        else if (row == 1 && column == gridSize.x)
        {
            soloSpawnTowerPositions.Add(towerPos);
        }
        else if (row == (int)(gridSize.y / 2) && column == 1)
        {
            soloSpawnTowerPositions.Add(towerPos);
        }
        else if (row == (int)(gridSize.y / 2) && column == gridSize.x)
        {
            soloSpawnTowerPositions.Add(towerPos);
        }
        else if (row == gridSize.y && column == 1)
        {
            soloSpawnTowerPositions.Add(towerPos);
        }
        else if (row == gridSize.y && column == (int)(gridSize.x / 2))
        {
            soloSpawnTowerPositions.Add(towerPos);
        }
        else if (row == gridSize.y && column == gridSize.x)
        {
            soloSpawnTowerPositions.Add(towerPos);
        }
    }

    private void DeleteWalls()
    {
        Debug.Log("deleteWalls");
        SetDictionary();
        Debug.Log(cells.Count);

        for (int i = 0; i < cells.Count; i++)
        {

            int second = UnityEngine.Random.Range(0, 100);

            if (second > 75)
            {
                cells[i].RemoveAt(1);
                cells[i].RemoveAt(0);

                continue;
            }

            int choice = UnityEngine.Random.Range(0, cells[i].Count);

            cells[i].RemoveAt(choice);
        }

        SetDictionary();
    }

    private void HandleGameMode()
    {
        int index = 0;

        for (int i = 0; i < availableGameModes.Length; i++)
        {
            if (availableGameModes[i] == selectedGameMode)
            {
                index = i;
                break;
            }
        }

        this.GetComponent<PhotonView>().RPC("GameModeObjectOff", RpcTarget.AllBuffered, index);
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
            if (wallInfo.position.x == 0 || wallInfo.position.x == gridSize.x - 1)
            {
                newWallInfo.isTouchingBorder = true;
            }
        }
        else if (wallInfo.orientation == WallType.WallOrientation.Vertical)
        {
            if (wallInfo.position.y == 0 || wallInfo.position.y == gridSize.y - 1)
            {
                newWallInfo.isTouchingBorder = true;
            }
        }

        return newWallInfo.isTouchingBorder;
    }

    private void SetDictionary()
    {
        walls.Clear();

        for (int i = 0; i < cells.Count; ++i)
        {
            for (int ii = 0; ii < cells[i].Count; ++ii)
            {
                walls.Add(cells[i][ii]);
            }
        }

        Debug.Log(walls.Count);
    }

    private bool IsWithinGrid(Vector2 pos)
    {
        if (pos.x == -1 || pos.x == gridSize.x || pos.y == -1 || pos.y == gridSize.y)
        {
            return false;
        }

        return true;
    }

    private Vector2 GetPositionInDirection(Vector2 original, Direction direction)
    {
        Vector2 newPos = new Vector2(original.x, original.y);

        if (direction == Direction.left)
        {
            newPos.x -= 1;
        }
        else if (direction == Direction.right)
        {
            newPos.x += 1;
        }
        else if (direction == Direction.up)
        {
            newPos.y -= 1;
        }
        else if (direction == Direction.down)
        {
            newPos.y += 1;
        }

        if (IsWithinGrid(newPos))
        {
            //Debug.Log($"Got position {newPos} to the {direction} of {original}");
            return newPos;
        }

        return original;
    }

    private Dictionary<Direction, WallInfo> GetSurroundingWallInfo(Vector2 original)
    {
        Dictionary<Direction, WallInfo> result = new Dictionary<Direction, WallInfo>();


        WallInfo right = new WallInfo()
        {
            position = new Vector2(original.x, original.y),
            orientation = WallType.WallOrientation.Vertical
        };
        right.isTouchingBorder = GetWallBool(right);
        WallInfo left = new WallInfo()
        {
            position = new Vector2(original.x - 1, original.y),
            orientation = WallType.WallOrientation.Vertical
        };
        left.isTouchingBorder = GetWallBool(left);
        WallInfo up = new WallInfo()
        {
            position = new Vector2(original.x, original.y - 1),
            orientation = WallType.WallOrientation.Horizontal
        };
        up.isTouchingBorder = GetWallBool(up);
        WallInfo down = new WallInfo()
        {
            position = new Vector2(original.x, original.y),
            orientation = WallType.WallOrientation.Horizontal
        };
        down.isTouchingBorder = GetWallBool(down);

        result.Add(Direction.right, right);
        result.Add(Direction.left, left);
        result.Add(Direction.up, up);
        result.Add(Direction.down, down);

        //Debug.Log($"At position {original} there are {result.Count} possibilities");

        return result;
    }

    private List<Vector2> FloodFillCheck()
    {
        List<Vector2> positionsToVisit = new List<Vector2>();
        List<Vector2> visited = new List<Vector2>();

        positionsToVisit.Add(Vector2.zero);

        while (positionsToVisit.Count > 0)
        {
            if (!visited.Contains(positionsToVisit[0]))
            {
                Dictionary<Direction, WallInfo> surroundingWalls = GetSurroundingWallInfo(positionsToVisit[0]);

                for (int i = 0; i < surroundingWalls.Count; i++)
                {
                    if (!walls.Contains(surroundingWalls[directions[i]]))
                    {
                        Vector2 newLocation = GetPositionInDirection(positionsToVisit[0], directions[i]);
                        if (newLocation != positionsToVisit[0] && !positionsToVisit.Contains(newLocation))
                        {
                            //Debug.Log($"{directions[i]} has no wall... adding {GetPositionInDirection(positionsToVisit[0], directions[i])}");
                            positionsToVisit.Add(GetPositionInDirection(positionsToVisit[0], directions[i]));
                        }
                    }

                }
            }

            visited.Add(positionsToVisit[0]);
            positionsToVisit.RemoveAt(0);
        }

        List<Vector2> allRequiredPosition = new List<Vector2>();
        List<Vector2> boxedIn = new List<Vector2>();

        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                allRequiredPosition.Add(new Vector2(i, j));
            }
        }

        for (int i = 0; i < allRequiredPosition.Count; i++)
        {
            if (!visited.Contains(allRequiredPosition[i]))
            {
                //Debug.LogWarning($"Does not contain {allRequiredPosition[i]}.");
                boxedIn.Add(allRequiredPosition[i]);
            }

        }

        return boxedIn;
    }

    private void GuaranteePlayability()
    {
        List<List<WallInfo>> listsOfInfo = new List<List<WallInfo>>();
        List<List<WallInfo>> allSegements = new List<List<WallInfo>>();

        List<WallInfo> checkedWalls = new List<WallInfo>();
        SetDictionary();

        for (int wallCount = 0; wallCount < walls.Count; wallCount++)
        {
            WallInfo newWallInfo = walls[wallCount];

            listsOfInfo.Add(GetPossibleInfo(newWallInfo));

            List<WallInfo> segment = new List<WallInfo>() { walls[wallCount] };



            while (listsOfInfo.Count > 0)
            {
                for (int ii = 0; ii < listsOfInfo[0].Count; ii++)
                {
                    if (walls.Contains(listsOfInfo[0][ii]))
                    {
                        if (!segment.Contains(walls[wallCount]) && IsWithinGrid(listsOfInfo[0][ii].position))
                        {
                            segment.Add(listsOfInfo[0][ii]);

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

            foreach (WallInfo list in segment)
            {
                checkedWalls.Add(list);
            }
        }

        while (FloodFillCheck().Count > 0)
        {
            List<Vector2> boxedPositions = FloodFillCheck();

            RemoveBoxes(boxedPositions[UnityEngine.Random.Range(0, boxedPositions.Count)]);
        }

        Debug.LogWarning("flood fill check complete");

    }

    private void RemoveBoxes(Vector2 boxedPosition)
    {
        Dictionary<Direction, WallInfo> surroundingWalls = GetSurroundingWallInfo(boxedPosition);

        for (int dir = 0; dir < surroundingWalls.Count; dir++)
        {
            if (walls.Contains(surroundingWalls[directions[dir]]))
            {
                walls.Remove(surroundingWalls[directions[dir]]);
            }
        }
    }

    private void DoPhysicalMap()
    {
        for (int i = 0; i < cellContainer.transform.childCount; i++)
        {
            for (int ii = 0; ii < cellContainer.transform.GetChild(i).childCount; ii++)
            {
                WallInfo check = new WallInfo() { orientation = cellContainer.transform.GetChild(i).GetChild(ii).gameObject.GetComponent<MapWall>().type, position = cellContainer.transform.GetChild(i).GetChild(ii).gameObject.GetComponent<MapWall>().position };
                check.isTouchingBorder = GetWallBool(check);

                if (!walls.Contains(check))
                {
                    
                    cellContainer.transform.GetChild(i).GetChild(ii).gameObject.GetComponent<MapWall>().Destroy();
                }
            }
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("mapGeneration"))
        {
            Debug.Log("map has been generated");
            OnMapGenerated?.Invoke();
        }
    }

    [PunRPC]
    
    void GameModeObjectOff(int gameModeIndex)     
    {
        Debug.LogError("punRPC");
        selectedGameMode = availableGameModes[gameModeIndex];

        if (selectedGameMode != null)
        {
            for (int i = 0; i < spawnTowers.Count; i++)
            {
                spawnTowers[i].SetActive(selectedGameMode.HasTeams);
                if (PhotonNetwork.IsMasterClient)
                {
                    spawnTowers[i].transform.localPosition = towerPositions[i];
                }
            }

            for (int i = 0; i < teamFlags.Count; i++)
            {
                teamFlags[i].SetActive(selectedGameMode.HasFlag);
                safeZoneObjects[i].SetActive(selectedGameMode.HasFlag);
                if (PhotonNetwork.IsMasterClient)
                {
                    teamFlags[i].transform.localPosition = flagPositions[i];
                }
            }

            controlZoneObject.SetActive(selectedGameMode.HasZones);
            Debug.Log($"{selectedGameMode.Name} has zones: {selectedGameMode.HasZones}");

            if (!selectedGameMode.HasTeams && PhotonNetwork.IsMasterClient)
            {
                List<GameObject> soloTowers = new List<GameObject>();
                Debug.Log(soloSpawnTowerPositions.Count);
                Debug.Log(soloSpawnTowerPrefabs.Count);

                for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                {
                    if (PhotonNetwork.PlayerList[i].GetPhotonTeam() != null)
                    {
                        Debug.Log((int)PhotonNetwork.PlayerList[i].GetPhotonTeam().Code - 1);
                        GameObject newTower = PhotonNetwork.Instantiate(soloSpawnTowerPrefabs[(int)PhotonNetwork.PlayerList[i].GetPhotonTeam().Code - 1].name, soloSpawnTowerPositions[soloTowerOrder[i]], Quaternion.identity);

                        soloTowers.Add(newTower);
                    }
                    else
                    {
                        Debug.Log($"{PhotonNetwork.PlayerList[i].NickName} is not on a team");
                    }
                }

                OnSoloTowersGen?.Invoke(soloTowers);
            }

        }
        else
        {
            Debug.Log("no game mode");
        }
    }
}
