using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using System;
using Photon.Realtime;

public class PhotonConnector : MonoBehaviourPunCallbacks
{

    public static Action GetPhotonFriends = delegate { };
    private void Start()
    {
        string nickname = PlayerPrefs.GetString("USERNAME");
        ConnectToPhoton(nickname);
    }

    private void Awake()
    {
        UIInvite.OnRoomInviteAccept += HandleRoomInviteAccept;
    }

    private void OnDestroy()
    {
        UIInvite.OnRoomInviteAccept -= HandleRoomInviteAccept;
    }

    private void CreatePhotonRoom(string roomName)
    {
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 4;
        PhotonNetwork.JoinOrCreateRoom(roomName, ro, TypedLobby.Default);
    }

    private void HandleRoomInviteAccept(string roomName)
    {
        PlayerPrefs.SetString("PHOTONROOM", roomName);
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            if (PhotonNetwork.InLobby)
            {
                JoinPlayerRoom();
            }
        }
    }

    private void JoinPlayerRoom()
    {
        string roomName = PlayerPrefs.GetString("PHOTONROOM");
        PhotonNetwork.JoinRoom(roomName);
        PlayerPrefs.SetString("PHOTONROOM", "");
    }

    private void ConnectToPhoton(string nickName)
    {

        Debug.Log($"Connect to Photon as {nickName}");
        PhotonNetwork.AuthValues = new AuthenticationValues(nickName);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = nickName;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log($"You have connected to the Photon Master Server");
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log($"You have connected to a Photon Lobby");
        GetPhotonFriends?.Invoke();
        string roomName = PlayerPrefs.GetString("PHOTONROOM");
        if (!string.IsNullOrEmpty(roomName))
        {
            JoinPlayerRoom();
        }
    }

    public void OnCreateRoomClicked(string roomName)
    {
        CreatePhotonRoom(roomName);
    }
}
