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
    private GameMode selectedGameMode;

    Player player;

    public List<int> teamScores = new List<int>();

    public static Action<PhotonTeam> OnRoundWon = delegate { };

    // Start is called before the first frame update
    private void Awake()
    {
        GameManager.OnSetTeams += HandleStartGame;
        TankHealth.OnOutOfHearts += HandleDeadTank;
        FlagSafeZone.OnTeamReachedSafeZone += HandleFlagPoint;
    }

    // Update is called once per frame
    private void OnDestroy()
    {
        GameManager.OnSetTeams -= HandleStartGame;
        TankHealth.OnOutOfHearts -= HandleDeadTank;
        FlagSafeZone.OnTeamReachedSafeZone -= HandleFlagPoint;
    }

    private void HandleStartGame(GameMode gm)
    {
        selectedGameMode = gm;
        teamScores.Clear();

        for (int i = 0; i < (selectedGameMode.MaxPlayers / selectedGameMode.TeamSize); ++i)
        {
            teamScores.Add(0);
        }
    }

    private void HandleZonePoint(int teamIndex, int score)
    {
        //teamScores[teamIndex - 1]++;
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
                    for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                    {
                        if (PhotonNetwork.PlayerList[i].GetPhotonTeam() != player.GetPhotonTeam())
                        {
                            teamScores[player.GetPhotonTeam().Code - 1]++;
                            OnRoundWon?.Invoke(PhotonNetwork.PlayerList[i].GetPhotonTeam());
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
    }
}
