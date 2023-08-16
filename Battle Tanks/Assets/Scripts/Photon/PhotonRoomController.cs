using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using TMPro;

public class PhotonRoomController : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameMode selectedGameMode;
    [SerializeField] private GameMode[] availableGameModes;
    [SerializeField] TMP_InputField createInput;

    int numberOfRounds;
    

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

    public void OnClickCreate(GameMode gameMode)
    {
        if (createInput.text.Length >= 1)
        {
            selectedGameMode = gameMode;
            CreatePhotonRoom(createInput.text);
        }
    }

    public void StartGame()
    {
        //Hashtable setTeams = new Hashtable() { { "GAMEMODE", currentSelectedGameMode.Name } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "TEAMS", "0" } });
        Debug.Log("startGame");
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
        ro.MaxPlayers = selectedGameMode.MaxPlayers;

        string[] roomProperties = { GAME_MODE };

        Hashtable customRoomProperties = new Hashtable()
        { {GAME_MODE, selectedGameMode.name} };

        ro.CustomRoomPropertiesForLobby = roomProperties;
        ro.CustomRoomProperties = customRoomProperties;

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

    #region Photon Callbacks
    public override void OnJoinedRoom()
    {
        selectedGameMode = GetRoomGameMode();
        OnJoinRoom?.Invoke(selectedGameMode);
        OnRoomStatusChange?.Invoke(PhotonNetwork.InRoom);
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
            OnStartGame?.Invoke();
        }


    }
    #endregion
}
