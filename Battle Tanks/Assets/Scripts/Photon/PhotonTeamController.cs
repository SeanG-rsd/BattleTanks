using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PhotonTeamController : MonoBehaviourPunCallbacks
{
    [SerializeField] private List<PhotonTeam> roomTeams;
    [SerializeField] private int teamSize;
    [SerializeField] private PhotonTeam priorTeam;

    public static Action<List<PhotonTeam>, GameMode> OnCreateTeams = delegate { };
    public static Action<Player, PhotonTeam> OnSwitchTeam = delegate { };
    public static Action<Player> OnRemovePlayer = delegate { };
    public static Action OnClearTeams = delegate { };

    private void Awake()
    {
        UITeam.OnSwitchToTeam += HandleSwitchTeam;
        PhotonRoomController.OnGameSettingsSelected += HandleCreateTeams;
        PhotonRoomController.OnRoomLeft += HandleLeaveRoom;
        PhotonRoomController.OnOtherPlayerLeftRoom += HandleOtherPlayerLeftRoom;
        PhotonRoomController.OnStartGame += HandleStartGame;

        roomTeams = new List<PhotonTeam>();
    }

    private void OnDestroy()
    {
        UITeam.OnSwitchToTeam -= HandleSwitchTeam;
        PhotonRoomController.OnGameSettingsSelected -= HandleCreateTeams;
        PhotonRoomController.OnRoomLeft -= HandleLeaveRoom;
        PhotonRoomController.OnOtherPlayerLeftRoom -= HandleOtherPlayerLeftRoom;
        PhotonRoomController.OnStartGame -= HandleStartGame;
    }

    private void HandleSwitchTeam(PhotonTeam newTeam)
    {
        if (PhotonNetwork.LocalPlayer.GetPhotonTeam() == null)
        {
            priorTeam = PhotonNetwork.LocalPlayer.GetPhotonTeam();
            PhotonNetwork.LocalPlayer.JoinTeam(newTeam);
        }
        else if (CanSwitchToTeam(newTeam))
        {
            priorTeam = PhotonNetwork.LocalPlayer.GetPhotonTeam();
            PhotonNetwork.LocalPlayer.SwitchTeam(newTeam);
        }
    }

    private void HandleCreateTeams(GameMode gameMode, int numRounds)
    {
        CreateTeams(gameMode);

        OnCreateTeams?.Invoke(roomTeams, gameMode);

        AutoAssignPlayerToTeam(PhotonNetwork.LocalPlayer, gameMode);
    }

    private void HandleLeaveRoom()
    {
        PhotonNetwork.LocalPlayer.LeaveCurrentTeam();
        roomTeams.Clear();
        teamSize = 0;
        OnClearTeams?.Invoke();
    }

    private void HandleOtherPlayerLeftRoom(Player otherPlayer)
    {
        OnRemovePlayer?.Invoke(otherPlayer);
    }

    private void HandleStartGame()
    {
        PhotonNetwork.LocalPlayer.CustomProperties["playerTeam"] = (int)PhotonNetwork.LocalPlayer.GetPhotonTeam().Code;
        Debug.Log("set player team");
    }

    private void CreateTeams(GameMode gameMode)
    {
        teamSize = gameMode.TeamSize;
        int numberOfTeams = gameMode.MaxPlayers;

        if (gameMode.HasTeams)
        {
            numberOfTeams = gameMode.MaxPlayers / gameMode.TeamSize;
        }

        for (int i = 1; i <= numberOfTeams; i++)
        {
            roomTeams.Add(new PhotonTeam
            {
                Name = $"Team {i}",
                Code = (byte)i
            });
        }
    }

    private bool CanSwitchToTeam(PhotonTeam newTeam)
    {
        bool canSwitch = false;

        if (PhotonNetwork.LocalPlayer.GetPhotonTeam().Code != newTeam.Code)
        {
            Player[] players = null;
            if (PhotonTeamsManager.Instance.TryGetTeamMembers(newTeam.Code, out players))
            {
                if (players.Length < teamSize)
                {
                    canSwitch = true;
                }
            }
        }

        return canSwitch;
    }

    private void AutoAssignPlayerToTeam(Player player, GameMode gameMode)
    {
        foreach (PhotonTeam team in roomTeams)
        {
            int teamPlayerCount = PhotonTeamsManager.Instance.GetTeamMembersCount(team.Code);

            if (teamPlayerCount < gameMode.TeamSize)
            {
                if (player.GetPhotonTeam() == null)
                {
                    player.JoinTeam(team.Code);
                }
                else if (player.GetPhotonTeam().Code != team.Code)
                {
                    player.SwitchTeam(team.Code);
                }
                break;
            }
        }
    }


    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        object teamCodeObject;
        if (changedProps.TryGetValue(PhotonTeamsManager.TeamPlayerProp, out teamCodeObject))
        {
            if (teamCodeObject == null) return;

            byte teamCode = (byte)teamCodeObject;

            PhotonTeam newTeam;
            if (PhotonTeamsManager.Instance.TryGetTeamByCode(teamCode, out newTeam))
            {
                OnSwitchTeam?.Invoke(targetPlayer, newTeam);
            }
        }
    }
}
