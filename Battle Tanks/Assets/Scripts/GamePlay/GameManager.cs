using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using System;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPunCallbacks
{
    
    public static Action<Vector2, GameMode> OnGenerateMap = delegate { };
    public static Action<GameMode> OnSetTeams = delegate { };

    [SerializeField] private GameMode[] possibleGameModes;
    private GameMode selectedGameMode;

    private Vector2 mapSize;

    Player player;

    [SerializeField] private Vector2[] possibleMapSizes;

    [SerializeField] private Camera deadCam;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;

    private void Awake()
    {
        TankHealth.OnOutOfHearts += HandleDeadTank;
    }

    private void OnDestroy()
    {
        TankHealth.OnOutOfHearts -= HandleDeadTank;
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

    private void HandleDeadTank(Tank tank)
    {
        if (tank.GetComponent<PhotonView>().IsMine)
        {
            tank.gameObject.GetComponent<TankMovement>().PlayerCamera.SetActive(false);
            tank.Destroy();
            //Display.displays[1].Activate();
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log(targetPlayer.NickName);
        UpdateTankProperties(targetPlayer);
        Debug.Log("playerPropertiesUpdate");
    }

    void UpdateTankProperties(Player player)
    {
        if (player.CustomProperties.ContainsKey("aliveState"))
        {
            if ((int)player.CustomProperties["aliveState"] == 0)
            {
                Debug.Log("a tank has died");
                Player[] teamMates;
                player.TryGetTeamMates(out teamMates);


                bool localTeamState = false;

                if (teamMates.Length > 0)
                {
                    for (int i = 0; i < teamMates.Length; i++)
                    {
                        if ((int)teamMates[i].CustomProperties["aliveState"] == 1)
                        {
                            localTeamState = true;
                            break;
                        }
                    }
                }

                if ((int)player.CustomProperties["aliveState"] == 1)
                {
                    localTeamState = true;
                }

                if (!localTeamState)
                {
                    Debug.Log($"Team {player.GetPhotonTeam().Code} has no more alive tanks on it.");
                    if (player.GetPhotonTeam().Code != PhotonNetwork.LocalPlayer.GetPhotonTeam().Code)
                    {
                        winScreen.SetActive(true);
                    }
                    else
                    {
                        loseScreen.SetActive(true);
                    }
                }
            }
            else
            {
                Debug.Log($"{player.NickName} is still alive");
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Hashtable hashtable = new Hashtable() { { "aliveState", 0} };
            Debug.Log("set custom prop");

            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
        }
    }
}
