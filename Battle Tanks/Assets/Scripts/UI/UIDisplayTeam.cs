using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIDisplayTeam : MonoBehaviour
{
    [SerializeField] private UITeam uiTeamPrefab;
    [SerializeField] private List<UITeam> uiTeams;
    [SerializeField] private Transform teamContainer;

    public static Action<Player, PhotonTeam> OnAddPlayerToTeam = delegate { };
    public static Action<Player> OnRemovePlayerFromTeam = delegate { };

    private void Awake()
    {
        PhotonTeamController.OnCreateTeams += HandleCreateTeams;
        PhotonTeamController.OnSwitchTeam += HandleSwitchTeam;
        PhotonTeamController.OnRemovePlayer += HandleRemovePlayer;
        PhotonTeamController.OnClearTeams += HandleClearTeams;
        uiTeams = new List<UITeam>();
    }

    private void OnDestroy()
    {
        PhotonTeamController.OnCreateTeams -= HandleCreateTeams;
        PhotonTeamController.OnSwitchTeam -= HandleSwitchTeam;
        PhotonTeamController.OnRemovePlayer -= HandleRemovePlayer;
        PhotonTeamController.OnClearTeams -= HandleClearTeams;
    }

    private void HandleCreateTeams(List<PhotonTeam> teams, GameMode gameMode)
    {
        foreach(PhotonTeam team in teams)
        {
            UITeam uiTeam = Instantiate(uiTeamPrefab, teamContainer);
            uiTeam.Initialize(team, gameMode.TeamSize);
            uiTeams.Add(uiTeam);
        }
    }

    private void HandleSwitchTeam(Player player, PhotonTeam newTeam)
    {
        OnRemovePlayerFromTeam?.Invoke(player);

        OnAddPlayerToTeam?.Invoke(player, newTeam);
    }

    private void HandleRemovePlayer(Player player)
    {
        OnRemovePlayerFromTeam?.Invoke(player);
    }

    private void HandleClearTeams()
    {
        foreach (UITeam team in uiTeams)
        {
            Destroy(team.gameObject);
        }

        uiTeams.Clear();
    }
}
