using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;
using System;
using Unity.VisualScripting;

public class WinCheck : MonoBehaviourPunCallbacks
{
    public GameMode selectedGameMode;

    Player player;

    public List<int> teamScores = new List<int>();

    public static Action<PhotonTeam> OnRoundWon = delegate { };
    public static Action OnWinCheckReady = delegate { };
    public static Action<Player, ExitGames.Client.Photon.Hashtable> OnZonePoint = delegate { };

    [SerializeField] private UIGameModeScore gameModeScore;

    public int zonePointsToWin;

    private bool zoneReady;

    // Start is called before the first frame update
    private void Awake()
    {
        GameManager.OnSetTeams += HandleStartGame;
        TankHealth.OnOutOfHearts += HandleDeadTank;
        FlagSafeZone.OnTeamReachedSafeZone += HandleFlagPoint;
        ControlZone.OnTeamGainControlPoint += HandleZonePoint;
        RoundManager.OnRoundStarted += HandleRoundStart;
    }

    // Update is called once per frame
    private void OnDestroy()
    {
        GameManager.OnSetTeams -= HandleStartGame;
        TankHealth.OnOutOfHearts -= HandleDeadTank;
        FlagSafeZone.OnTeamReachedSafeZone -= HandleFlagPoint;
        ControlZone.OnTeamGainControlPoint -= HandleZonePoint;
        RoundManager.OnRoundStarted -= HandleRoundStart;
    }

    private void HandleRoundStart()
    {
        zoneReady = true;
        gameModeScore.SetScoreBar();
    }

    private void HandleStartGame(GameMode gm)
    {
        Debug.Log("handle start game");

        selectedGameMode = gm;
        teamScores.Clear();

        for (int i = 0; i < (selectedGameMode.MaxPlayers / selectedGameMode.TeamSize); ++i)
        {
            teamScores.Add(0);
        }

        Debug.Log("on win check ready");
        gameModeScore.HandleSetup();
    }

    private void HandleZonePoint(int teamIndex)
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            Debug.Log($"handle zone point for team {teamIndex} with name {PhotonNetwork.LocalPlayer.NickName}");

            ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable() { { "zonePoint", teamIndex } };

            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
        }
    }

    private void HandleFlagPoint(int teamIndex)
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            Debug.Log("handle flag point");
            ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable() { { "flagPoint", teamIndex } };

            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
        }
    }

    private void HandleDeadTank(Tank tank)
    {
        if (tank.GetComponent<PhotonView>().IsMine)
        {
            tank.gameObject.GetComponent<TankMovement>().PlayerCamera.SetActive(false);

            ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable() { { "aliveState", 0 } };
            Debug.Log("set custom prop");

            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        UpdateTankProperties(targetPlayer, changedProps);
    }

    void UpdateTankProperties(Player player, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("aliveState") && selectedGameMode.HasHearts)
        {
            if ((int)player.CustomProperties["aliveState"] == 0)
            {
                List<Player> playerFromEachTeam = new List<Player>();

                for (int i = 0; i < PhotonNetwork.PlayerList.Length; ++i)
                {
                    if (playerFromEachTeam.Count == 0)
                    {
                        playerFromEachTeam.Add(PhotonNetwork.PlayerList[i]);
                    }
                    else
                    {
                        bool moveOn = false;

                        for (int j = 0; j < playerFromEachTeam.Count; ++j)
                        {
                            if (playerFromEachTeam[j].GetPhotonTeam() == PhotonNetwork.PlayerList[i].GetPhotonTeam())
                            {
                                moveOn = true;
                                break;
                            }
                        }

                        if (!moveOn)
                        {
                            playerFromEachTeam.Add(PhotonNetwork.PlayerList[i]);
                        }
                    }


                }

                Debug.Log($"{playerFromEachTeam.Count} Teams were found");

                Debug.Log("a tank has died");
                List<bool> teamStates = new List<bool>();

                for (int i = 0; i < playerFromEachTeam.Count; ++i)
                {
                    Player[] teamMates;
                    playerFromEachTeam[i].TryGetTeamMates(out teamMates);


                    bool localTeamState = false; // if false the player's team is dead

                    if (teamMates.Length > 0)
                    {
                        for (int j = 0; j < teamMates.Length; j++)
                        {
                            if ((int)teamMates[j].CustomProperties["aliveState"] == 1)
                            {
                                localTeamState = true;
                                break;
                            }
                        }
                    }

                    if ((int)playerFromEachTeam[i].CustomProperties["aliveState"] == 1)
                    {
                        localTeamState = true;
                    }

                    teamStates.Add(localTeamState);
                }

                bool hasOneAliveTeam = false;
                bool noTeamsAlive = true;

                int index = 0;

                for (int i = 0; i < teamStates.Count; ++i)
                {
                    Debug.Log($"{playerFromEachTeam[i].NickName}'s Team is {teamStates[i]}");

                    if (!hasOneAliveTeam && teamStates[i])
                    {
                        noTeamsAlive = false;
                        hasOneAliveTeam = true;
                        index = playerFromEachTeam[i].GetPhotonTeam().Code;
                    }
                    else if (hasOneAliveTeam && teamStates[i])
                    {
                        index = 0;
                        break;
                    }
                }

                Debug.Log(index);

                if (index != 0 || noTeamsAlive)
                {
                    Debug.Log("someone has won the round");

                    for (int i = 0; i < playerFromEachTeam.Count; ++i)
                    {
                        if (playerFromEachTeam[i].GetPhotonTeam().Code == index)
                        {
                            OnRoundWon?.Invoke(playerFromEachTeam[i].GetPhotonTeam());
                        }
                    }
                }
            }
            else
            {
                Debug.Log($"{player.NickName} is still alive");
            }
        }

        if (changedProps.ContainsKey("flagPoint") && selectedGameMode.HasFlag)
        {
            //teamScores[teamIndex - 1]++;
            Debug.Log("flag won round");

            for (int i = 0; i < PhotonNetwork.PlayerList.Length; ++i)
            {
                if (PhotonNetwork.PlayerList[i].GetPhotonTeam().Code == (int)player.CustomProperties["flagPoint"])
                {
                    
                    OnRoundWon?.Invoke(PhotonNetwork.PlayerList[i].GetPhotonTeam());
                    break;
                }
            }
        }

        if (changedProps.ContainsKey("zonePoint") && selectedGameMode.HasZones)
        {
            Debug.Log("zone point earned");

            for (int i = 0; i < PhotonNetwork.PlayerList.Length; ++i)
            {
                PhotonNetwork.PlayerList[i].CustomProperties["zonePoint"] = player.CustomProperties["zonePoint"];

                if (PhotonNetwork.PlayerList[i].GetPhotonTeam().Code == (int)player.CustomProperties["zonePoint"])
                {
                    if (!PhotonNetwork.PlayerList[i].CustomProperties.ContainsKey("zoneScore"))
                    {
                        Debug.Log("set score");
                        PhotonNetwork.PlayerList[i].CustomProperties["zoneScore"] = 0;
                    }

                    
                    ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable() { { "zoneScore", (int)PhotonNetwork.PlayerList[i].CustomProperties["zoneScore"] + 1 } };

                    PhotonNetwork.PlayerList[i].SetCustomProperties(hashtable);
                }
            }
        }

        if (changedProps.ContainsKey("zoneScore") && selectedGameMode.HasZones)
        {
            Debug.Log($" Team {player.GetPhotonTeam().Code} has a score of {player.CustomProperties["zoneScore"]}");
            gameModeScore.SetScoreBar();

            
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; ++i)
            {                
                if (PhotonNetwork.PlayerList[i].GetPhotonTeam().Code == player.GetPhotonTeam().Code)
                {
                    
                    if ((int)player.CustomProperties["zoneScore"] >= zonePointsToWin && zoneReady)
                    {
                        OnRoundWon?.Invoke(PhotonNetwork.PlayerList[i].GetPhotonTeam());
                        zoneReady = false;
                        Debug.Log("zone round won");
                        Debug.Log(player.CustomProperties["zoneScore"]);
                        break;
                    }
                    
                }
            }
        }
    }
}
