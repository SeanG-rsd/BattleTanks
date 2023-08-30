using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System;

public class LeaveGame : MonoBehaviour
{
    public static Action OnLeaveGame = delegate { };

    [SerializeField] private GameObject quitScreen;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && GetComponent<PhotonView>().IsMine)
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
            OnLeaveGame();
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.LocalPlayer.LeaveCurrentTeam();
            PhotonNetwork.LeaveRoom();
        }
    }
}
