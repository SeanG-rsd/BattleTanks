//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Runtime.InteropServices.WindowsRuntime;
using ExitGames.Client.Photon;
using Photon.Pun.UtilityScripts;
using System;


public class RoomController : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameMode selectedGameMode;
    [SerializeField] private GameMode[] availableGameModes;
    private const string GAME_MODE = "GAMEMODE";

    public static Action<bool> OnRoomStatusChange = delegate { };
    public static Action<GameMode> OnJoinRoom = delegate { };
    public static Action OnRoomLeft = delegate { };
    public static Action<Player> OnOtherPlayerLeftRoom = delegate { };


    public TMP_InputField createInput;

    public GameObject lobbyPanel;
    public GameObject roomPanel;
    public GameObject gameModePanel;
    public GameObject waitForPanel;

    public GameObject waitForPlayersPanel;
    public TMP_Text waitForPlayersText;

    public TMP_Text roomName;

    public RoomItem roomItemPrefab;
    List<RoomItem> roomItemsList = new List<RoomItem>();
    public Transform contentObject;

    public float timeBetweenUpdate = 1.5f;
    float nextUpdateTime;

    public List<PlayerItem> playerItemsList = new List<PlayerItem>();
    public PlayerItem playerItemPrefab;
    public Transform playerItemParent;

    public GameObject playButton;
    public GameObject chooseGameButton;

    public GameObject teamDeathmatch;
    public GameObject captureTheFlag;
    public GameObject zoneControl;
    public GameObject freeForAll;

    ExitGames.Client.Photon.Hashtable playerPropeties = new ExitGames.Client.Photon.Hashtable();

    public bool choseGame = false;

    public int minPlayers;

    public void OnClickCreate()
    {
        if (createInput.text.Length >= 1)
        {
            PhotonNetwork.CreateRoom(createInput.text, new RoomOptions() { MaxPlayers = 4, BroadcastPropsChangeToAll = true });

        }
    }

    void UpdateRoomList(List<RoomInfo> list)
    {
        foreach (RoomItem item in roomItemsList)
        {
            Destroy(item.gameObject);
        }
        roomItemsList.Clear();

        foreach (RoomInfo room in list)
        {
            RoomItem newRoom = Instantiate(roomItemPrefab, contentObject);
            newRoom.SetRoomName(room.Name);
            roomItemsList.Add(newRoom);
        }
    }

    public void JoinPhotonRoom(string roomName)
    {
        Hashtable expectedCustomRoomProperties = new Hashtable()
        { {GAME_MODE, selectedGameMode.Name } };

        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    public void OnClickLeaveRoom()
    {

        PhotonNetwork.LeaveRoom();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    void UpdatePlayerList()
    {
        foreach (PlayerItem item in playerItemsList)
        {
            Destroy(item.gameObject);
        }
        playerItemsList.Clear();

        if (PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayItem = Instantiate(playerItemPrefab, playerItemParent);
            newPlayItem.SetPlayInfo(player.Value);

            if (player.Value == PhotonNetwork.LocalPlayer)
            {
                newPlayItem.ApplyLocalChanges();
            }

            playerItemsList.Add(newPlayItem);
        }
    }

    

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= minPlayers)
        {

            if (!gameModePanel.activeSelf && !choseGame)
            {
                gameModePanel.SetActive(true);
                playButton.SetActive(false);
                waitForPanel.SetActive(false);
            }

            if (gameModePanel.activeSelf)
            {
                chooseGameButton.SetActive(true);
            }
            else
            {
                chooseGameButton.SetActive(false);
            }
        }
        else
        {
            gameModePanel.SetActive(false);
            playButton.SetActive(false);
            chooseGameButton.SetActive(false);
            
        }

        if (!choseGame && !PhotonNetwork.IsMasterClient && !waitForPanel.activeSelf)
        {
            waitForPanel.SetActive(true);
        }
        else if (!PhotonNetwork.IsMasterClient && waitForPanel.activeSelf && choseGame)
        {
            Debug.Log("choseGame");
            waitForPanel.SetActive(false);
        }

        if (PhotonNetwork.CurrentRoom != null)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount < minPlayers)
            {
                waitForPlayersPanel.SetActive(true);
                if (minPlayers - PhotonNetwork.CurrentRoom.PlayerCount > 1)
                {
                    waitForPlayersText.text = "Waiting For " + (minPlayers - PhotonNetwork.CurrentRoom.PlayerCount) + " Players";
                }
                else if (minPlayers - PhotonNetwork.CurrentRoom.PlayerCount == 1)
                {
                    waitForPlayersText.text = "Waiting For " + (minPlayers - PhotonNetwork.CurrentRoom.PlayerCount) + " Player";
                }
            }
            else
            {
                waitForPlayersPanel.SetActive(false);
            }
        }
    }

    public void OnClickPlayGameButton()
    {
        PhotonNetwork.LoadLevel("Game");
    }

    public void OnClickChooseGameButton()
    {
        playButton.SetActive(true);
        chooseGameButton.SetActive(false);
        gameModePanel.SetActive(false);
        playerPropeties["choseGame"] = 1;
        PhotonNetwork.SetPlayerCustomProperties(playerPropeties);
        choseGame = true;
    }

    private GameMode GetRoomGameMode()
    {
        string gameModeName = (string)PhotonNetwork.CurrentRoom.CustomProperties[GAME_MODE];
        GameMode gameMode = null;

        for (int i = 0; i < availableGameModes.Length; i++)
        {
            if (string.Compare(availableGameModes[i].Name, gameModeName) == 0)
            {
                gameMode = availableGameModes[i];
                break;
            }
        }
        return gameMode;
    }

    private void CreatePhotonRoom()
    {
        string roomName = Guid.NewGuid().ToString();
        RoomOptions ro = GetRoomOptions();

        PhotonNetwork.JoinOrCreateRoom(roomName, ro, TypedLobby.Default);
    }

    private RoomOptions GetRoomOptions()
    {
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = selectedGameMode.MaxPlayers;

        string[] roomProperties = { GAME_MODE };

        Hashtable customRomProperties = new Hashtable()
        { {GAME_MODE, selectedGameMode.Name}};

        ro.CustomRoomPropertiesForLobby = roomProperties;
        ro.CustomRoomProperties = customRomProperties;

        return ro;
    }

    private void DebugPlayerList()
    {
        string players = "";
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            players += $"{player.Value.NickName}, ";
        }
        Debug.Log($"Current Room Players: {players}");
    }

    private void HandleLobbyJoined()
    {
        string roomName = PlayerPrefs.GetString("PHOTONROOM");
        if (!string.IsNullOrEmpty(roomName))
        {
            PhotonNetwork.JoinRoom(roomName);
            PlayerPrefs.SetString("PHOTONROOM", "");
        }
    }

    private void HandleLeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            OnRoomLeft?.Invoke();
            PhotonNetwork.LeaveRoom();
        }
    }

    private void HandleGetRoomStatus()
    {
        OnRoomStatusChange?.Invoke(PhotonNetwork.InRoom);
    }

    private void HandleGameModeSelected(GameMode gameMode)
    {
        if (!PhotonNetwork.IsConnectedAndReady) return;
        
        selectedGameMode = gameMode;
        JoinPhotonRoom("adf");
    }


    #region Photon Callbacks
    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = "Room: " + PhotonNetwork.CurrentRoom.Name;

        selectedGameMode = GetRoomGameMode();
        OnJoinRoom?.Invoke(selectedGameMode);
        OnRoomStatusChange?.Invoke(PhotonNetwork.InRoom);

        UpdatePlayerList();
    }

    public override void OnLeftRoom()
    {
        selectedGameMode = null;
        OnRoomStatusChange?.Invoke(PhotonNetwork.InRoom);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (Time.time >= nextUpdateTime)
        {
            UpdateRoomList(roomList);
            nextUpdateTime = Time.time + timeBetweenUpdate;
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreatePhotonRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    #endregion
}
