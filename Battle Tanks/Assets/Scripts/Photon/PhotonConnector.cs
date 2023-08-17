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
    public static Action OnRejoinLobby = delegate { };

    [SerializeField] private UIDisplayRoom uiRoom;
    private void Start()
    {
        Debug.Log("start");

        if (PhotonNetwork.InRoom)
        {
            Debug.Log("is in room");
            uiRoom.LeaveRoom();
        }
        if (!PhotonNetwork.InRoom)
        {

            ConnectToPhoton(nickName);
        }
       
    }

    private void Awake()
    {
        nickName = PlayerPrefs.GetString("USERNAME");
    }

    private void ConnectToPhoton(string nickName)
    {
        Debug.Log($"Connect to Photon as {nickName}");
        PhotonNetwork.AuthValues = new AuthenticationValues(nickName);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = nickName;
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            
        }
        else
        {
            Debug.Log("player is already connected");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log($"You have connected to the Photon Master Server");
        if (!PhotonNetwork.InLobby && !PhotonNetwork.InRoom)
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
        Debug.LogWarning("leftRoom");
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("connecting to master");
    }
}
