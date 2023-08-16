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

    [SerializeField] private string nickName;
    public static Action GetPhotonFriends = delegate { };
    public static Action OnLobbyJoined = delegate { };
    private void Start()
    {
        if (!PhotonNetwork.InRoom && !PhotonNetwork.InLobby)
        {
            ConnectToPhoton(nickName);
        }
        else if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveRoom();
        }
        Debug.Log(PhotonNetwork.InRoom);
        Debug.Log(PhotonNetwork.InLobby);
        Debug.Log(PhotonNetwork.IsConnectedAndReady);
    }

    private void Awake()
    {
        nickName = PlayerPrefs.GetString("USERNAME");
    }

    private void OnDestroy()
    {
        //UIInvite.OnRoomInviteAccept -= HandleRoomInviteAccept;
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
        OnLobbyJoined?.Invoke();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
}
