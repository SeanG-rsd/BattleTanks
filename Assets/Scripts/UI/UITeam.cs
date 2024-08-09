using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using TMPro;
using System;
using UnityEngine.UI;

public class UITeam : MonoBehaviour
{
    [SerializeField] private int teamSize;
    [SerializeField] private int maxTeamSize;
    [SerializeField] private PhotonTeam team;
    [SerializeField] private TMP_Text teamNameText;
    [SerializeField] private Image teamImage;
    [SerializeField] private Transform playerSelectionContainer;
    [SerializeField] private UIPlayerSelection playerSelectionPrefab;
    [SerializeField] private Dictionary<Player, UIPlayerSelection> playerSelections;

    private TeamInfo teamInfo;

    public static Action<PhotonTeam> OnSwitchToTeam = delegate { };

    private void Awake()
    {

        UIDisplayTeam.OnAddPlayerToTeam += HandleAddPlayerToTeam;
        UIDisplayTeam.OnRemovePlayerFromTeam += HandleRemovePlayerFromTeam;
        PhotonRoomController.OnRoomLeft += HandleLeaveRoom;
    }

    private void OnDestroy()
    {
        UIDisplayTeam.OnAddPlayerToTeam -= HandleAddPlayerToTeam;
        UIDisplayTeam.OnRemovePlayerFromTeam -= HandleRemovePlayerFromTeam;
        PhotonRoomController.OnRoomLeft -= HandleLeaveRoom;
    }

    public void Initialize(PhotonTeam team, int teamSize)
    {
        teamInfo = FindObjectOfType<TeamInfo>();
        this.team = team;
        maxTeamSize = teamSize;
        playerSelections = new Dictionary<Player, UIPlayerSelection>();
        UpdateTeamUI();

        Player[] teamMembers;
        if (PhotonTeamsManager.Instance.TryGetTeamMembers(this.team.Code, out teamMembers))
        {
            foreach (Player player in teamMembers)
            {
                AddPlayerToTeam(player);
            }
        }
    }

    public void HandleAddPlayerToTeam(Player player, PhotonTeam team)
    {
        if (this.team.Code == team.Code)
        {
            AddPlayerToTeam(player);
        }
    }

    public void HandleRemovePlayerFromTeam(Player player)
    {
        RemovePlayerFromTeam(player);
    }

    private void HandleLeaveRoom()
    {
        Destroy(gameObject);
    }

    private void UpdateTeamUI()
    {
        teamNameText.SetText($"{teamInfo.teamNames[team.Code - 1]} : {playerSelections.Count} / {maxTeamSize}");

        teamImage.color = teamInfo.teamColors[team.Code - 1];
    }

    private void AddPlayerToTeam(Player player)
    {
        UIPlayerSelection uiPlayerSelection = Instantiate(playerSelectionPrefab, playerSelectionContainer);
        uiPlayerSelection.Initialize(player);
        playerSelections.Add(player, uiPlayerSelection);
        UpdateTeamUI();
    }

    private void RemovePlayerFromTeam(Player player)
    {
        if (playerSelections.ContainsKey(player))
        {
            Destroy(playerSelections[player].gameObject);
            playerSelections.Remove(player);
            UpdateTeamUI();
        }
    }

    public void SwitchToTeam()
    {
        if (teamSize >= maxTeamSize) return;

        OnSwitchToTeam?.Invoke(team);
    }
}
