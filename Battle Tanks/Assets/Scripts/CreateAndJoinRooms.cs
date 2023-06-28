using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Runtime.InteropServices.WindowsRuntime;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{

    public TMP_InputField createInput;

    public GameObject lobbyPanel;
    public GameObject roomPanel;
    public GameObject gameModePanel;
    public GameObject waitForPanel;

    public GameObject waitForPlayersPanel;
    public TMP_Text waitForPlayersText;

    public TMP_Text roomName;

    public RoomItem roomItemPrefab;
    List<RoomItem> roomItemsList = new List<RoomItem>();
    public Transform contentObject;

    public float timeBetweenUpdate = 1.5f;
    float nextUpdateTime;

    public List<PlayerItem> playerItemsList = new List<PlayerItem>();
    public PlayerItem playerItemPrefab;
    public Transform playerItemParent;

    public GameObject playButton;
    public GameObject chooseGameButton;

    public GameObject teamDeathmatch;
    public GameObject captureTheFlag;
    public GameObject zoneControl;
    public GameObject freeForAll;

    ExitGames.Client.Photon.Hashtable playerPropeties = new ExitGames.Client.Photon.Hashtable();

    public bool choseGame = false;

    public int minPlayers;

    public void OnClickCreate()
    {
        if (createInput.text.Length >= 1)
        {
            PhotonNetwork.CreateRoom(createInput.text, new RoomOptions() { MaxPlayers = 4, BroadcastPropsChangeToAll = true });

        }
    }

    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = "Room: " + PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (Time.time >= nextUpdateTime)
        {
            UpdateRoomList(roomList);
            nextUpdateTime = Time.time + timeBetweenUpdate;
        }
    }

    void UpdateRoomList(List<RoomInfo> list)
    {
        foreach (RoomItem item in roomItemsList)
        {
            Destroy(item.gameObject);
        }
        roomItemsList.Clear();

        foreach (RoomInfo room in list)
        {
            RoomItem newRoom = Instantiate(roomItemPrefab, contentObject);
            newRoom.SetRoomName(room.Name);
            roomItemsList.Add(newRoom);
        }
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnClickLeaveRoom()
    {

        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    void UpdatePlayerList()
    {
        foreach (PlayerItem item in playerItemsList)
        {
            Destroy(item.gameObject);
        }
        playerItemsList.Clear();

        if (PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayItem = Instantiate(playerItemPrefab, playerItemParent);
            newPlayItem.SetPlayInfo(player.Value);

            if (player.Value == PhotonNetwork.LocalPlayer)
            {
                newPlayItem.ApplyLocalChanges();
            }

            playerItemsList.Add(newPlayItem);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= minPlayers)
        {

            if (!gameModePanel.activeSelf && !choseGame)
            {
                gameModePanel.SetActive(true);
                playButton.SetActive(false);
                waitForPanel.SetActive(false);
            }

            if (gameModePanel.activeSelf)
            {
                chooseGameButton.SetActive(true);
            }
            else
            {
                chooseGameButton.SetActive(false);
            }
        }
        else
        {
            gameModePanel.SetActive(false);
            playButton.SetActive(false);
            chooseGameButton.SetActive(false);
            
        }

        if (!choseGame && !PhotonNetwork.IsMasterClient && !waitForPanel.activeSelf)
        {
            waitForPanel.SetActive(true);
        }
        else if (!PhotonNetwork.IsMasterClient && waitForPanel.activeSelf && choseGame)
        {
            Debug.Log("choseGame");
            waitForPanel.SetActive(false);
        }

        if (PhotonNetwork.CurrentRoom != null)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount < minPlayers)
            {
                waitForPlayersPanel.SetActive(true);
                if (minPlayers - PhotonNetwork.CurrentRoom.PlayerCount > 1)
                {
                    waitForPlayersText.text = "Waiting For " + (minPlayers - PhotonNetwork.CurrentRoom.PlayerCount) + " Players";
                }
                else if (minPlayers - PhotonNetwork.CurrentRoom.PlayerCount == 1)
                {
                    waitForPlayersText.text = "Waiting For " + (minPlayers - PhotonNetwork.CurrentRoom.PlayerCount) + " Player";
                }
            }
            else
            {
                waitForPlayersPanel.SetActive(false);
            }
        }
    }

    public void OnClickPlayGameButton()
    {
        PhotonNetwork.LoadLevel("Game");
    }

    public void OnClickChooseGameButton()
    {
        playButton.SetActive(true);
        chooseGameButton.SetActive(false);
        gameModePanel.SetActive(false);
        playerPropeties["choseGame"] = 1;
        PhotonNetwork.SetPlayerCustomProperties(playerPropeties);
        choseGame = true;
    }

    public void OnClickTeamDeathmatch()
    {
        
        
    }

    public void OnClickCaptureTheFlag()
    {
        
        
    }

    public void OnClickZoneControl()
    {
        
        

    }

    public void OnClickFreeForAll()
    {
        
        
    }
}
