using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using System;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.UI;

public class RoundManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private int inBetweenTime;
    [SerializeField] private GameObject roundScreen;
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private TMP_Text roundTimeText;

    public static Action OnGameStarted = delegate { };
    public static Action OnRoundStarted = delegate { };

    private float roundTimer;

    private int roundNumber;
    private int numberOfRounds;

    [Header("---Round Win Screen---")]
    [SerializeField] private float winScreenTime;
    private float winTimer;
    [SerializeField] private GameObject RoundWinScreen;
    [SerializeField] private TeamInfo teamInfo;
    [SerializeField] private TMP_Text winTeamText;
    [SerializeField] private TMP_Text yourTeamScoreText;
    [SerializeField] private TMP_Text opposingTeamScoreText;
    [SerializeField] private Image yourTeamImage;
    [SerializeField] private Image opposingTeamImage;
    [SerializeField] private Image winningTeamImage;

    [SerializeField] private PhotonView view;

    private PhotonTeam currentlyWon;

    private void Awake()
    {
        GameManager.OnStartGame += HandleStartGame;
        GameManager.OnGenerateMap += HandleGenerateMap;
        GameManager.OnRoundWon += HandleRoundWon;
    }

    private void OnDestroy()
    {
        GameManager.OnStartGame -= HandleStartGame;
        GameManager.OnGenerateMap -= HandleGenerateMap;
        GameManager.OnRoundWon -= HandleRoundWon;
    }

    private void Update()
    {
        if (roundScreen.activeSelf)
        {
            roundTimer -= Time.deltaTime;
            roundTimeText.text = Mathf.RoundToInt(roundTimer).ToString();

            if (roundTimer < 0)
            {
                roundScreen.SetActive(false);
                OnGameStarted?.Invoke();
            }
        }

        if (RoundWinScreen.activeSelf)
        {
            winTimer -= Time.deltaTime;

            if (winTimer < 0)
            {
                winTimer = winScreenTime;
                RoundWinScreen.SetActive(false);
                HandleNewRound();
            }
        }
    }

    private void HandleStartGame()
    {
        PhotonNetwork.LocalPlayer.CustomProperties["teamRoundScore"] = 0;
        HandleNewRound();
    }

    private void HandleNewRound()
    {
        currentlyWon = null;
        roundScreen.SetActive(true);
        roundTimer = inBetweenTime;
        OnRoundStarted?.Invoke();
    }

    private void HandleGenerateMap(Vector2 size, GameMode gm)
    {
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "roundNumber", 1 } });
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("roundNumber"))
        {
            UpdateRoundNumber((int)propertiesThatChanged["roundNumber"]);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer == PhotonNetwork.LocalPlayer)
        {
            if (changedProps.ContainsKey("teamRoundScore"))
            {
                UpdateWinScreen(currentlyWon);
            }
        }
    }

    private void UpdateRoundNumber(int number)
    {
        roundText.SetText($"Round {number}");
    }

    private void UpdateWinScreen(PhotonTeam winningTeam)
    {
        if (view.IsMine)
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; ++i)
            {
                if (PhotonNetwork.PlayerList[i].GetPhotonTeam() != PhotonNetwork.LocalPlayer.GetPhotonTeam())
                {
                    opposingTeamImage.color = teamInfo.teamColors[PhotonNetwork.PlayerList[i].GetPhotonTeam().Code - 1];
                    opposingTeamScoreText.text = PhotonNetwork.PlayerList[i].CustomProperties["teamRoundScore"].ToString();
                    break;
                }
            }
        }

        winningTeamImage.color = teamInfo.teamColors[winningTeam.Code - 1];
        winTeamText.text = $"{teamInfo.teamNames[winningTeam.Code - 1]} Team Won the Round!";

        yourTeamImage.color = teamInfo.teamColors[PhotonNetwork.LocalPlayer.GetPhotonTeam().Code - 1];
        yourTeamScoreText.text = PhotonNetwork.LocalPlayer.CustomProperties["teamRoundScore"].ToString();
    }

    private void HandleRoundWon(PhotonTeam team)
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; ++i)
        {
            if (PhotonNetwork.PlayerList[i].GetPhotonTeam() == team)
            {
                Debug.Log($"{PhotonNetwork.PlayerList[i].NickName} has won a round");
                if (PhotonNetwork.PlayerList[i].CustomProperties.ContainsKey("teamRoundScore"))
                {
                    PhotonNetwork.PlayerList[i].CustomProperties["teamRoundScore"] = (int)PhotonNetwork.PlayerList[i].CustomProperties["teamRoundScore"] + 1;   
                }
                else
                {
                    PhotonNetwork.PlayerList[i].CustomProperties["teamRoundScore"] = 1;
                }
                Debug.Log($"{PhotonNetwork.PlayerList[i].NickName}'s score is now {PhotonNetwork.PlayerList[i].CustomProperties["teamRoundScore"]}");
            }
        }

        int roundNumber = (int)PhotonNetwork.CurrentRoom.CustomProperties["roundNumber"] + 1;

        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "roundNumber", roundNumber } });
        RoundWinScreen.SetActive(true);
        winTimer = winScreenTime;
        UpdateWinScreen(team);
        currentlyWon = team;
    }
}
