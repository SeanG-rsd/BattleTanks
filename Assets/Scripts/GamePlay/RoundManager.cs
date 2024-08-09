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
using System.Linq;
using PlayFab.AuthenticationModels;

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

    [SerializeField] private GameObject teamScoreLine;
    [SerializeField] private GameObject soloScoreLine;

    [SerializeField] private WinCheck winCheck;

    private List<KeyValuePair<Player, int>> scoresForSolo;
    [SerializeField] private GameObject soloScoreGameObject;
    [SerializeField] private Transform soloScoreBoard;

    [SerializeField] private GameObject joysticks;

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

                joysticks.SetActive(true);
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
        ResetFlags();
        OnRoundStarted?.Invoke();
    }

    private void SetAliveStateForAll()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            PhotonNetwork.PlayerList[i].CustomProperties["aliveState"] = 1;
            PhotonNetwork.PlayerList[i].CustomProperties["zoneScore"] = 0;
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
        if (winCheck.selectedGameMode.HasTeams)
        {
            UpdateScoreLine();
        }
        else
        {
            UpdateSoloUI();
        }

        winningTeamImage.color = teamInfo.teamColors[winningTeam.Code - 1];
        winTeamText.text = $"{teamInfo.teamNames[winningTeam.Code - 1]} Team Won the Round!";
    }

    private void UpdateSoloScoreLine()
    {
        List<Player> playerFromEachTeam = new List<Player>();
        Dictionary<Player, int> playerWithScore = new Dictionary<Player, int>();

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; ++i)
        {
            if (playerFromEachTeam.Count == 0)
            {
                playerFromEachTeam.Add(PhotonNetwork.PlayerList[i]);
                if (PhotonNetwork.PlayerList[i].CustomProperties.ContainsKey("teamRoundScore"))
                {
                    playerWithScore.Add(PhotonNetwork.PlayerList[i], (int)PhotonNetwork.PlayerList[i].CustomProperties["teamRoundScore"]);
                }
                else
                {
                    playerWithScore.Add(PhotonNetwork.PlayerList[i], 0);
                }
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
                    playerWithScore.Add(PhotonNetwork.PlayerList[i], (int)PhotonNetwork.PlayerList[i].CustomProperties["teamRoundScore"]);
                }
            }
        }

        scoresForSolo = playerWithScore.ToList(); // list of keyvalue pairs <Player, int>

        scoresForSolo.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));       
    }

    private void UpdateSoloUI()
    {
        //Debug.Log("solo ui");

        soloScoreLine.SetActive(true);
        teamScoreLine.SetActive(false);

        int index = 0;

        if (scoresForSolo.Count > 3)
        {
            index = 3;
        }
        else
        {
            index = scoresForSolo.Count;
        }

        for (int i = 0; i < soloScoreBoard.childCount; ++i)
        {
            Destroy(soloScoreBoard.GetChild(i).gameObject);
        }

        //Debug.Log($"index is {index}");

        for (int i = index - 1; i >= 0; --i)
        {
            GameObject score = Instantiate(soloScoreGameObject, new Vector3(0, 0, 0), Quaternion.identity);
            score.transform.SetParent(soloScoreBoard);

            score.transform.localScale = Vector3.one;

            SoloScore soloScore = score.GetComponent<SoloScore>();

            soloScore.SetImage(i, teamInfo.teamColors[scoresForSolo[i].Key.GetPhotonTeam().Code - 1]);
            soloScore.SetScore(scoresForSolo[i].Value);
        }
    }

    private void UpdateScoreLine()
    {
        soloScoreLine.SetActive(false);
        teamScoreLine.SetActive(true);

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; ++i)
        {
            if (PhotonNetwork.PlayerList[i].GetPhotonTeam() != PhotonNetwork.LocalPlayer.GetPhotonTeam())
            {
               // Debug.Log($"{PhotonNetwork.PlayerList[i].NickName} is not on the same team");
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

        PhotonTeam winningTeam;

        if (winCheck.selectedGameMode.HasTeams)
        {
            winningTeam = GetLeadingTeam();
        }
        else
        {
            winningTeam = scoresForSolo[scoresForSolo.Count - 1].Key.GetPhotonTeam();
        }

        winningTeamImage.color = teamInfo.teamColors[winningTeam.Code - 1];
        winTeamText.text = $"{teamInfo.teamNames[winningTeam.Code - 1]} Team Won the Game!";
    }

    private void HandleRoundWon(PhotonTeam team)
    {
        joysticks.SetActive(false);

        currentRoundNumber++;

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; ++i)
        {
            if (PhotonNetwork.PlayerList[i].GetPhotonTeam() == team)
            {
                //Debug.Log($"{PhotonNetwork.PlayerList[i].NickName} has won a round");
                if (PhotonNetwork.PlayerList[i].CustomProperties.ContainsKey("teamRoundScore"))
                {
                    PhotonNetwork.PlayerList[i].CustomProperties["teamRoundScore"] = (int)PhotonNetwork.PlayerList[i].CustomProperties["teamRoundScore"] + 1;   
                }
                else
                {
                    PhotonNetwork.PlayerList[i].CustomProperties["teamRoundScore"] = 1;
                }
                //Debug.Log($"{PhotonNetwork.PlayerList[i].NickName}'s score is now {PhotonNetwork.PlayerList[i].CustomProperties["teamRoundScore"]}");
            }
        }

        if (currentRoundNumber > numberOfRounds && winCheck.selectedGameMode.HasTeams)
        {
            GameOverScreen();
            return;
        }
        else if (!winCheck.selectedGameMode.HasTeams)
        {
            UpdateSoloScoreLine();

            if (scoresForSolo[scoresForSolo.Count - 1].Value >= 3)
            {
                GameOverScreen();
                return;
            }
        }

        UpdateRoundNumber(currentRoundNumber);
        RoundWinScreen.SetActive(true);
        winTimer = winScreenTime;
        UpdateWinScreen(team);
        currentlyWon = team;
    }
}
