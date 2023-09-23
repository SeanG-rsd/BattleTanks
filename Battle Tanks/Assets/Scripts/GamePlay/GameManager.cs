using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using System;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;
using Unity.VisualScripting;

public class GameManager : MonoBehaviourPunCallbacks
{
    
    public static Action<Vector2, GameMode> OnGenerateMap = delegate { };
    public static Action<GameMode> OnSetTeams = delegate { };

    public static Action OnStartGame = delegate { };

    [SerializeField] private GameMode[] possibleGameModes;
    private GameMode selectedGameMode;

    private Vector2 mapSize;

    Player player;

    [SerializeField] private Vector2[] possibleMapSizes;

    [SerializeField] private Camera deadCam;

    [SerializeField] private GameObject topDownCam;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = false;

        TankHealth.OnOutOfHearts += HandleDeadTank;
        MapGeneator.OnMapGenerated += HandleStartGame;
        
    }

    private void OnDestroy()
    {
        TankHealth.OnOutOfHearts -= HandleDeadTank;
        MapGeneator.OnMapGenerated -= HandleStartGame;
    }

    private void Start()
    {
        SetMapSize();
        GetGameMode();

        if (PhotonNetwork.IsMasterClient)
        {
            OnGenerateMap?.Invoke(mapSize, selectedGameMode);
        }
        
        OnSetTeams?.Invoke(selectedGameMode);
    }

    private void SetMapSize()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount <= 4)
        {
            mapSize = possibleMapSizes[0];
        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount > 4 && PhotonNetwork.CurrentRoom.PlayerCount <= 6)
        {
            mapSize = possibleMapSizes[1];
        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount > 6)
        {
            mapSize = possibleMapSizes[2];
        }
    }

    private void GetGameMode()
    {
        for (int i = 0; i < possibleGameModes.Length; i++)
        {
            if ((string)PhotonNetwork.CurrentRoom.CustomProperties["GAMEMODE"] == possibleGameModes[i].Name)
            {
                selectedGameMode = possibleGameModes[i];
                return;
            }
        }
    }

    private void HandleStartGame()
    {
        OnStartGame?.Invoke();
        
    }

    private void HandleDeadTank(Tank tank)
    {
        topDownCam.SetActive(true);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {

        Debug.Log("playerLeftRoom");
        Tank[] tanks = FindObjectsOfType<Tank>();
        Debug.Log(tanks.Length);

        foreach (Tank tank in tanks)
        {
            if (tank.view.ViewID == (int)otherPlayer.CustomProperties["TankViewID"])
            {
                PhotonNetwork.Destroy(tank.gameObject);
                Debug.Log(otherPlayer.CustomProperties["TankViewID"]);
            }
        }
    }
}

