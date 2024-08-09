using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIDisplayRoom : MonoBehaviour
{
    [SerializeField] private TMP_Text roomGameModeText;
    [SerializeField] private TMP_Text roomRoundNumText;
    [SerializeField] private GameObject exitButton;
    [SerializeField] private GameObject roomContainer;
    [SerializeField] private GameObject[] hideObjects;
    [SerializeField] private GameObject[] showObjects;

    [SerializeField] GameObject lobbyPanel;
    [SerializeField] GameObject roomPanel;
    [SerializeField] GameObject masterPanel;
    [SerializeField] GameObject waitForPanel;

    [SerializeField] TMP_Text roomName;
    [SerializeField] UIGameMode gameModePrefab;

    [SerializeField] GameObject playButton;
    [SerializeField] GameObject chooseGameButton;

    [SerializeField] TMP_Text playerNameText;
    [SerializeField] GameObject tankSelectScreen;

    public static Action OnLeaveRoom = delegate { };

    private void Awake()
    {
        tankSelectScreen.SetActive(false);
        PhotonRoomController.OnGameSettingsSelected += HandleGameModeSelected;
        PhotonRoomController.OnJoinRoom += HandleJoinRoom;
        PhotonRoomController.OnRoomLeft += HandleRoomLeft;
        
    }

    private void OnDestroy()
    {
        PhotonRoomController.OnGameSettingsSelected += HandleGameModeSelected;
        PhotonRoomController.OnJoinRoom -= HandleJoinRoom;
        PhotonRoomController.OnRoomLeft -= HandleRoomLeft;
    }

    private void HandleJoinRoom(GameMode gameMode)
    {
        roomName.SetText(PhotonNetwork.CurrentRoom.Name);
        playerNameText.SetText(PhotonNetwork.LocalPlayer.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            masterPanel.SetActive(true);
            waitForPanel.SetActive(false);
        }
        else
        {
            waitForPanel.SetActive(true);
            masterPanel.SetActive(false);
        }

        foreach (GameObject obj in hideObjects)
        {
            obj.SetActive(!obj.activeSelf);
        }
    }

    private void HandleRoomLeft()
    {
        foreach (GameObject obj in showObjects)
        {
            obj.SetActive(!obj.activeSelf);
        }
    }

    private void HandleGameModeSelected(GameMode gameMode, int numRounds)
    {
        tankSelectScreen.SetActive(true);
        masterPanel.SetActive(false);
        waitForPanel.SetActive(false);
        hideObjects[0].SetActive(true);
        roomGameModeText.SetText(gameMode.ModeName);
        roomRoundNumText.SetText(PhotonNetwork.CurrentRoom.CustomProperties["NUMBEROFROUNDS"].ToString() + ((int)PhotonNetwork.CurrentRoom.CustomProperties["NUMBEROFROUNDS"] > 1 ? " Rounds" : " Round"));
        chooseGameButton.SetActive(false);
        if (PhotonNetwork.IsMasterClient)
        {
            playButton.SetActive(true);
        }
    }

    public void LeaveRoom()
    {
        OnLeaveRoom?.Invoke();
    }
}
