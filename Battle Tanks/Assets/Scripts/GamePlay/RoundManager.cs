using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using System;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.UI;
using Photon.Pun.Demo.Cockpit;

public class RoundManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private int inBetweenTime;
    [SerializeField] private GameObject roundScreen;
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private TMP_Text roundTimeText;

    public static Action OnGameStarted = delegate { };
    public static Action OnRoundStarted = delegate { };

    private float roundTimer;

    private int currentRoundNumber;
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

    private bool waitingForPlayerToLeave;
    private bool gameOver;

    [SerializeField] private Flag[] flagObjects;

    [SerializeField] private GameObject returnToLobbyObject;

    public static Action OnGameOver = delegate { };

    private void Awake()
    {
        numberOfRounds = (int)PhotonNetwork.CurrentRoom.CustomProperties["NUMBEROFROUNDS"];

        GameManager.OnStartGame += HandleStartGame;
        WinCheck.OnRoundWon += HandleRoundWon;
    }

    private void OnDestroy()
    {
        GameManager.OnStartGame -= HandleStartGame;
        WinCheck.OnRoundWon -= HandleRoundWon;
    }

    private void Update()
    {
        if (roundScreen.activeSelf && !gameOver)
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

        if (Input.GetMouseButtonDown(0) && waitingForPlayerToLeave)
        {
            PhotonNetwork.AutomaticallySyncScene = false;
            OnGameOver?.Invoke();
        }
    }

    private void HandleStartGame()
    {
        PhotonNetwork.LocalPlayer.CustomProperties["teamRoundScore"] = 0;
        currentRoundNumber = 1;
        UpdateRoundNumber(currentRoundNumber);
        HandleNewRound();
    }

    private void HandleNewRound()
    {
        currentlyWon = null;
        roundScreen.SetActive(true);
        SetAliveStateForAll();
        roundTimer = inBetweenTime;
        OnRoundStarted?.Invoke();
    }

    private void SetAliveStateForAll()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            PhotonNetwork.PlayerList[i].CustomProperties["aliveState"] = 1;
        }
    }

    private void ResetFlags()
    {
        foreach (Flag flag in flagObjects)
        {
            flag.GoHome();
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("teamRoundScore"))
        {

            UpdateWinScreen(currentlyWon);
        }
    }

    private void UpdateRoundNumber(int number)
    {
        roundText.SetText($"Round {number}");
    }

    private void UpdateWinScreen(PhotonTeam winningTeam)
    {
        UpdateScoreLine();

        winningTeamImage.color = teamInfo.teamColors[winningTeam.Code - 1];
        winTeamText.text = $"{teamInfo.teamNames[winningTeam.Code - 1]} Team Won the Round!";
    }

    private void UpdateScoreLine()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; ++i)
        {
            if (PhotonNetwork.PlayerList[i].GetPhotonTeam() != PhotonNetwork.LocalPlayer.GetPhotonTeam())
            {
                Debug.Log($"{PhotonNetwork.PlayerList[i].NickName} is not on the same team");
                opposingTeamImage.color = teamInfo.teamColors[PhotonNetwork.PlayerList[i].GetPhotonTeam().Code - 1];
                if (PhotonNetwork.PlayerList[i].CustomProperties.ContainsKey("teamRoundScore"))
                {
                    opposingTeamScoreText.text = PhotonNetwork.PlayerList[i].CustomProperties["teamRoundScore"].ToString();
                }
                else
                {
                    opposingTeamScoreText.text = "0";
                }
                break;
            }
        }

        yourTeamImage.color = teamInfo.teamColors[PhotonNetwork.LocalPlayer.GetPhotonTeam().Code - 1];
        yourTeamScoreText.text = PhotonNetwork.LocalPlayer.CustomProperties["teamRoundScore"].ToString();
    }

    private PhotonTeam GetLeadingTeam()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; ++i)
        {
            if (PhotonNetwork.PlayerList[i].GetPhotonTeam() != PhotonNetwork.LocalPlayer.GetPhotonTeam())
            {
                if ((int)PhotonNetwork.PlayerList[i].CustomProperties["teamRoundScore"] > (int)PhotonNetwork.LocalPlayer.CustomProperties["teamRoundScore"])
                {
                    return PhotonNetwork.PlayerList[i].GetPhotonTeam();
                }
            }
        }

        return PhotonNetwork.LocalPlayer.GetPhotonTeam();
    }

    private void GameOverScreen()
    {
        UpdateScoreLine();
        returnToLobbyObject.SetActive(true);
        RoundWinScreen.SetActive(true);
        waitingForPlayerToLeave = true;
        gameOver = true;

        PhotonTeam winningTeam = GetLeadingTeam();

        winningTeamImage.color = teamInfo.teamColors[winningTeam.Code - 1];
        winTeamText.text = $"{teamInfo.teamNames[winningTeam.Code - 1]} Team Won the Game!";
    }

    private void HandleRoundWon(PhotonTeam team)
    {
        currentRoundNumber++;

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

        if (currentRoundNumber > numberOfRounds)
        {
            GameOverScreen();
            return;
        }

        UpdateRoundNumber(currentRoundNumber);
        RoundWinScreen.SetActive(true);
        winTimer = winScreenTime;
        UpdateWinScreen(team);
        currentlyWon = team;
    }
}
