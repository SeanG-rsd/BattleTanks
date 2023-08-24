using ExitGames.Client.Photon;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using TMPro;
using System.Globalization;
using System.Linq;
using UnityEngine.UI;

public class TankScoreboard : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject soloScoreLine;
    [SerializeField] private GameObject teamScoreLine;

    [SerializeField] private GameObject scoreBoard;

    [Header("-----Solo-----")]
    [SerializeField] private Transform soloScoreBoard;
    [SerializeField] private GameObject soloScoreGameObject;

    private List<KeyValuePair<Player, int>> scoresForSolo;
    [Header("-----Team-----")]
    [SerializeField] private TMP_Text opposingTeamScoreText;
    [SerializeField] private Image opposingTeamImage;

    [SerializeField] private TMP_Text yourTeamScoreText;
    [SerializeField] private Image yourTeamImage;

    private TeamInfo teamInfo;
    [SerializeField] private PhotonView view;
    private WinCheck winCheck;

    [SerializeField] private GameObject statLinePrefab;
    [SerializeField] private Transform statObject;

    private void Awake()
    {
        teamInfo = FindObjectOfType<TeamInfo>();
        winCheck = FindObjectOfType<WinCheck>();
    }
    private void Update()
    {
        if (view.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                UpdateSoloScoreLine();
                UpdateIndividualStats();
                scoreBoard.SetActive(true);

                if (winCheck.selectedGameMode.HasTeams)
                {
                    UpdateScoreLine();
                }
                else
                {
                    UpdateSoloUI();
                }
            }

            if (Input.GetKeyUp(KeyCode.Tab))
            {
                TurnOffUI();
            }
        }
    }

    private void TurnOffUI()
    {
        scoreBoard.SetActive(false);
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
                    if (PhotonNetwork.PlayerList[i].CustomProperties.ContainsKey("teamRoundScore"))
                    {
                        playerWithScore.Add(PhotonNetwork.PlayerList[i], (int)PhotonNetwork.PlayerList[i].CustomProperties["teamRoundScore"]);
                    }
                    else
                    {
                        playerWithScore.Add(PhotonNetwork.PlayerList[i], 0);
                    }
                }
            }
        }

        scoresForSolo = playerWithScore.ToList(); // list of keyvalue pairs <Player, int>

        scoresForSolo.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
    }

    private void UpdateSoloUI()
    {
        Debug.Log("solo ui");

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

        Debug.Log($"index is {index}");

        for (int i = index - 1; i >= 0; --i)
        {
            GameObject score = Instantiate(soloScoreGameObject, new Vector3(0, 0, 0), Quaternion.identity);
            score.transform.SetParent(soloScoreBoard);

            score.transform.localScale = Vector3.one;

            score.GetComponent<RectTransform>().sizeDelta = new Vector2(score.GetComponent<RectTransform>().sizeDelta.x, (soloScoreBoard.GetComponent<RectTransform>().sizeDelta.y / index));

            SoloScore soloScore = score.GetComponent<SoloScore>();

            soloScore.score.text = scoresForSolo[i].Value.ToString();
            soloScore.position.text = $"{index - i}.";
            soloScore.image.color = teamInfo.teamColors[scoresForSolo[i].Key.GetPhotonTeam().Code - 1];
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
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("teamRoundScore"))
        {
            yourTeamScoreText.text = PhotonNetwork.LocalPlayer.CustomProperties["teamRoundScore"].ToString();
        }
        else
        {
            yourTeamScoreText.text = "0";
        }
    }

    private void UpdateIndividualStats()
    {
        for (int i = 0; i < statObject.childCount; i++)
        {
            Destroy(statObject.GetChild(i).gameObject);
        }

        GameObject statline = Instantiate(statLinePrefab, Vector3.zero, Quaternion.identity);
        statline.transform.SetParent(statObject);
        statline.transform.localScale = Vector3.one;

        IndividualStatLine stat = SetStats(PhotonNetwork.LocalPlayer, statline);

        statline.GetComponent<IndividualStatLine>().name = stat.name;
        statline.GetComponent<IndividualStatLine>().kills = stat.kills;
        statline.GetComponent<IndividualStatLine>().assists = stat.assists;
        statline.GetComponent<IndividualStatLine>().deaths = stat.deaths;
        statline.GetComponent<IndividualStatLine>().team = stat.team;

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.LocalPlayer != PhotonNetwork.PlayerList[i])
            {
                GameObject stateline = Instantiate(statLinePrefab, Vector3.zero, Quaternion.identity);
                stateline.transform.SetParent(statObject);
                stateline.transform.localScale = Vector3.one;

                IndividualStatLine state = SetStats(PhotonNetwork.PlayerList[i], stateline);

                stateline.GetComponent<IndividualStatLine>().name = state.name;
                stateline.GetComponent<IndividualStatLine>().kills = state.kills;
                stateline.GetComponent<IndividualStatLine>().assists = state.assists;
                stateline.GetComponent<IndividualStatLine>().deaths = state.deaths;
                stateline.GetComponent<IndividualStatLine>().team = state.team;
            }
        }
    }

    private IndividualStatLine SetStats(Player player, GameObject stateline)
    {

        IndividualStatLine stat = stateline.GetComponent<IndividualStatLine>();

        if (player.CustomProperties.ContainsKey("KILLS"))
        {
            stat.kills.text = $"{player.CustomProperties["KILLS"]} K";
        }
        else
        {
            stat.kills.text = $"0 K";
        }

        if (player.CustomProperties.ContainsKey("DEATHS"))
        {
            stat.deaths.text = $"{player.CustomProperties["DEATHS"]} D";
        }
        else
        {
            stat.deaths.text = $"0 D";
        }

        if (player.CustomProperties.ContainsKey("ASSISTS"))
        {
            stat.assists.text = $"{player.CustomProperties["ASSISTS"]} A";
        }
        else
        {
            stat.assists.text = $"0 A";
        }

        stat._name.text = player.NickName;

        stat.team.color = teamInfo.teamColors[player.GetPhotonTeam().Code - 1];

        return stat;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("KILLS") || changedProps.ContainsKey("DEATHS") || changedProps.ContainsKey("ASSISTS"))
        {
            UpdateIndividualStats();
        }
    }
}
