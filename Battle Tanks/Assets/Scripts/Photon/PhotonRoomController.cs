using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public class PhotonRoomController : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameMode selectedGameMode;
    [SerializeField] private GameMode[] availableGameModes;
    private const string GAME_MODE = "GAMEMODE";

    public static Action<bool> OnRoomStatusChange = delegate { };
    public static Action<GameMode> OnJoinRoom = delegate { };
    public static Action OnRoomLeft = delegate { };
    public static Action<Player> OnOtherPlayerLeftRoom = delegate { };

    private void Awake()
    {
        //UIGameMode.OnGameModeSelected += HandleGameModeSelected;
        UIInvite.OnRoomInviteAccept += HandleRoomInviteAccept;
        PhotonConnector.OnLobbyJoined += HandleLobbyJoined;
        UIDisplayRoom.OnLeaveRoom += HandleLeaveRoom;
        UIFriend.OnGetRoomStatus += HandleGetRoomStatus;
    }

    private void OnDestroy()
    {
        //UIGameMode.OnGameModeSelected -= HandleGameModeSelected;
        UIInvite.OnRoomInviteAccept -= HandleRoomInviteAccept;
        PhotonConnector.OnLobbyJoined -= HandleLobbyJoined;
        UIDisplayRoom.OnLeaveRoom -= HandleLeaveRoom;
        UIFriend.OnGetRoomStatus -= HandleGetRoomStatus;
    }

    private void HandleGameModeSelected(GameMode gameMode)
    {
        if (!PhotonNetwork.IsConnectedAndReady) return;
        if (PhotonNetwork.InRoom) return;

        selectedGameMode = gameMode;
        JoinPhotonRoom();
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

    private void JoinPhotonRoom()
    {
        Hashtable expectedCustomProperties = new Hashtable()
        { { GAME_MODE, selectedGameMode.name} };

        PhotonNetwork.JoinRandomRoom(expectedCustomProperties, 0);
    }

    public void CreateRoom(GameMode gameMode)
    {
        selectedGameMode = gameMode;
        CreatePhotonRoom();
    }

    private void CreatePhotonRoom()
    {
        string roomName = Guid.NewGuid().ToString();
        RoomOptions ro = new RoomOptions();

        PhotonNetwork.JoinOrCreateRoom(roomName, ro, TypedLobby.Default);
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
        CreatePhotonRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        OnOtherPlayerLeftRoom?.Invoke(otherPlayer);
    }

    #endregion
}
