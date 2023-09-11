using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System;
using Photon.Realtime;

public class LeaveGame : MonoBehaviourPunCallbacks
{
    public static Action OnLeaveGame = delegate { };

    [SerializeField] private GameObject quitScreen;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            quitScreen.SetActive(!quitScreen.activeSelf);
        }
    }
    public void ClickLeaveGame()
    {
        HandleLeaveRoom();
    }

    private void HandleLeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            OnLeaveGame?.Invoke();
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.LocalPlayer.LeaveCurrentTeam();
            PhotonNetwork.LeaveRoom();
        }
    }
}
