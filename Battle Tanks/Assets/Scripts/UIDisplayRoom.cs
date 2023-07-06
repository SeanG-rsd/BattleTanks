using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIDisplayRoom : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TMP_Text roomGameModeText;
    [SerializeField] private GameObject exitButton;
    [SerializeField] private GameObject roomContainer;
    [SerializeField] private GameObject[] hideObjects;
    [SerializeField] private GameObject[] showObjects;

    public static Action OnLeaveRoom = delegate { };

    private void Awake()
    {
        RoomController.OnJoinRoom += HandleJoinRoom;
        RoomController.OnRoomLeft += HandleRoomLeft;
    }

    private void OnDestroy()
    {
        RoomController.OnJoinRoom -= HandleJoinRoom;
        RoomController.OnRoomLeft -= HandleRoomLeft;
    }

    private void HandleJoinRoom(GameMode gameMode)
    {
        roomGameModeText.SetText(PhotonNetwork.CurrentRoom.CustomProperties["GAMEMODE"].ToString());

        exitButton.SetActive(true);
        roomContainer.SetActive(true);

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

        foreach (GameObject obj in showObjects)
        {
            obj.SetActive(true);
        }
    }

    public void LeaveRoom()
    {
        OnLeaveRoom?.Invoke();
    }
}
