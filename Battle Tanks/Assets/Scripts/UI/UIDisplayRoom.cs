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
        roomGameModeText.SetText("WAITING");

        exitButton.SetActive(true);
        roomContainer.SetActive(true);
        lobbyPanel.SetActive(false);
        roomName.SetText(PhotonNetwork.CurrentRoom.Name);
        playerNameText.SetText(PhotonNetwork.LocalPlayer.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            masterPanel.SetActive(true);
        }
        else
        {
            waitForPanel.SetActive(true);
        }

        foreach (GameObject obj in hideObjects)
        {
            obj.SetActive(false);
        }
    }

    private void HandleRoomLeft()
    {
        roomGameModeText.SetText("JOINING ROOM");

        exitButton.SetActive(false);
        roomContainer.SetActive(false);
        lobbyPanel.SetActive(true);
        tankSelectScreen.SetActive(false);
        playButton.SetActive(false);

        masterPanel.SetActive(false);
        waitForPanel.SetActive(false);

        foreach (GameObject obj in showObjects)
        {
            obj.SetActive(true);
        }
    }

    private void HandleGameModeSelected(GameMode gameMode, int numRounds)
    {
        tankSelectScreen.SetActive(true);
        masterPanel.SetActive(false);
        waitForPanel.SetActive(false);
        roomGameModeText.SetText(PhotonNetwork.CurrentRoom.CustomProperties["GAMEMODE"].ToString());
        roomRoundNumText.SetText(PhotonNetwork.CurrentRoom.CustomProperties["NUMBEROFROUNDS"].ToString());
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
