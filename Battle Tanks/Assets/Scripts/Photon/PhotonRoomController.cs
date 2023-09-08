using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using TMPro;
using Photon.Chat;

public class PhotonRoomController : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameMode selectedGameMode;
    [SerializeField] private GameMode[] availableGameModes;
    [SerializeField] TMP_InputField createInput;

    private int numberOfRounds;

    [SerializeField] private UIRoomItem roomItemPrefab;
    private List<UIRoomItem> roomItemsList = new List<UIRoomItem>();
    [SerializeField] private Transform publicRoomList;
    

    private const string GAME_MODE = "GAMEMODE";

    public static Action<bool> OnRoomStatusChange = delegate { };
    public static Action<GameMode> OnJoinRoom = delegate { };
    public static Action OnRoomLeft = delegate { };
    public static Action<Player> OnOtherPlayerLeftRoom = delegate { };
    public static Action<GameMode, int> OnGameSettingsSelected = delegate { };
    public static Action OnStartGame = delegate { };

    private void Awake()
    {
        UIInvite.OnRoomInviteAccept += HandleRoomInviteAccept;
        PhotonConnector.OnLobbyJoined += HandleLobbyJoined;
        UIDisplayRoom.OnLeaveRoom += HandleLeaveRoom;
        UIFriend.OnGetRoomStatus += HandleGetRoomStatus;
    }

    private void OnDestroy()
    {
        UIInvite.OnRoomInviteAccept -= HandleRoomInviteAccept;
        PhotonConnector.OnLobbyJoined -= HandleLobbyJoined;
        UIDisplayRoom.OnLeaveRoom -= HandleLeaveRoom;
        UIFriend.OnGetRoomStatus -= HandleGetRoomStatus;
    }

    private void HandleRoomInviteAccept(string roomName)
    {
        PlayerPrefs.SetString("PHOTONROOM", roomName);
        if (PhotonNetwork.InRoom)
        {
            OnRoomLeft?.Invoke();
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinRoom(roomName);
                PlayerPrefs.SetString("PHOTONROOM", "");
            }
        }
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

    public void JoinPhotonRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnClickCreate()
    {
        if (createInput.text.Length >= 1 && PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InLobby)
        {
            CreatePhotonRoom(createInput.text);
        }
    }
    public void OnClickJoin()
    {
        if (createInput.text.Length >= 1 && PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InLobby)
        {
            CreatePhotonRoom(createInput.text);
        }
    }

    public void StartGame()
    {
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "TEAMS", "0" } });
    }

    private void CreatePhotonRoom(string name)
    {
        
        RoomOptions ro = new RoomOptions();
        ro = GetRoomOptions();

        PhotonNetwork.JoinOrCreateRoom(name, ro, TypedLobby.Default);
    }

    private RoomOptions GetRoomOptions()
    {
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 8;
        ro.CleanupCacheOnLeave = false;

        string[] roomProperties = { GAME_MODE };

        ro.CustomRoomPropertiesForLobby = roomProperties;

        return ro;
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

    private int GetRoomNumRound()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("NUMBEROFROUNDS"))
        {
            return (int)PhotonNetwork.CurrentRoom.CustomProperties["NUMBEROFROUNDS"];
        }
        return 0;
    }

    #region Photon Callbacks
    public override void OnJoinedRoom()
    {
        selectedGameMode = GetRoomGameMode();
        numberOfRounds = GetRoomNumRound();
        
        OnJoinRoom?.Invoke(selectedGameMode);
        OnRoomStatusChange?.Invoke(PhotonNetwork.InRoom);

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("GAMEMODESELECTED"))
        {
            if ((bool)PhotonNetwork.CurrentRoom.CustomProperties["GAMEMODESELECTED"])
            {
                OnGameSettingsSelected?.Invoke(selectedGameMode, numberOfRounds);
            }
        }
    }

    public override void OnLeftRoom()
    {
        selectedGameMode = null;
        OnRoomStatusChange?.Invoke(PhotonNetwork.InRoom);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreatePhotonRoom(Guid.NewGuid().ToString());
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        OnOtherPlayerLeftRoom?.Invoke(otherPlayer);
    }


    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {

        Debug.Log("some room property changed");

        if (propertiesThatChanged.ContainsKey("GAMEMODE") && propertiesThatChanged.ContainsKey("NUMBEROFROUNDS"))
        {
            for (int i = 0; i < availableGameModes.Length; i++)
            {
                if (availableGameModes[i].Name == propertiesThatChanged["GAMEMODE"].ToString())
                {
                    selectedGameMode = availableGameModes[i];
                    break;
                }
            }

            numberOfRounds = (int)propertiesThatChanged["NUMBEROFROUNDS"];

            OnGameSettingsSelected?.Invoke(selectedGameMode, numberOfRounds);
        }
        else if (propertiesThatChanged.ContainsKey("TEAMS"))
        {
            Debug.LogError("dflkajh");
            OnStartGame?.Invoke();
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("list update");
        UpdateRoomList(roomList);
    }

    private void UpdateRoomList(List<RoomInfo> list)
    {
        foreach (UIRoomItem item in roomItemsList)
        {
            Destroy(item.gameObject);
        }
        roomItemsList.Clear();

        foreach (RoomInfo room in list)
        {
            UIRoomItem roomItem = Instantiate(roomItemPrefab, publicRoomList);
            roomItem.SetRoomName(room.Name);
            roomItemsList.Add(roomItem);
        }
    }
    #endregion
}
