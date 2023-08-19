using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;

public class WinCheck : MonoBehaviourPunCallbacks
{
    private GameMode selectedGameMode;

    Player player;

    List<int> teamScores = new List<int>();

    // Start is called before the first frame update
    private void Awake()
    {
        GameManager.OnGenerateMap += HandleStartGame;
    }

    // Update is called once per frame
    private void OnDestroy()
    {
        GameManager.OnGenerateMap -= HandleStartGame;
    }

    private void HandleStartGame(Vector2 grid, GameMode gm)
    {
        selectedGameMode = gm;
    }

    private void HandleZonePoint(int teamIndex, int score)
    {

    }

    private void HandleFlagPoint(int teamIndex)
    {

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        UpdateTankProperties(targetPlayer);
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
                    for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                    {
                        if (PhotonNetwork.PlayerList[i].GetPhotonTeam() != player.GetPhotonTeam())
                        {
                            teamScores[player.GetPhotonTeam().Code - 1]++;
                        }
                    }
                }
            }
            else
            {
                Debug.Log($"{player.NickName} is still alive");
            }
        }
    }
}
