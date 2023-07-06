using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using System;
using Photon.Realtime;

public class ConnectToServer : MonoBehaviourPunCallbacks
{

    public TMP_InputField userNameInput;
    public TMP_Text buttonText;
    // Start is called before the first frame update
    private void Start()
    {
        string randomName = $"Tester{Guid.NewGuid().ToString()}";
        ConnectToPhoton(randomName);
    }

    private void ConnectToPhoton(string nickName)
    {
        Debug.Log($"Connect to Photon as {nickName}");
        PhotonNetwork.AuthValues = new AuthenticationValues(nickName);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = nickName;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnClickConnect()
    {
        if (userNameInput.text.Length >= 1)
        {
            PhotonNetwork.NickName = userNameInput.text;
            buttonText.text = "Connecting...";
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }
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
        
    }
}
