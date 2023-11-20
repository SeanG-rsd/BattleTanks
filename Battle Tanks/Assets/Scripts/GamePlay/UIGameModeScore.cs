using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;
using System;
using Unity.VisualScripting;
using UnityEngine.UI;
using TMPro;

public class UIGameModeScore : MonoBehaviourPunCallbacks
{
    private GameMode selectedGameMode;

    [SerializeField] private WinCheck winCheck;

    [SerializeField] private GameObject yourTeamScore;
    [SerializeField] private GameObject opposingTeamScore;

    [SerializeField] private Image yourTeamImage;
    [SerializeField] private Image opposingTeamImage;

    [SerializeField] private TMP_Text yourTeamText;
    [SerializeField] private TMP_Text opposingTeamText;

    [SerializeField] private TeamInfo teamInfo;

    [SerializeField] private int baseWidthOfTeam;
    [SerializeField] private int baseWidthOfBar;

    private int widthChange;

    public GameObject zoneBackground;

    private void Awake()
    {

        //WinCheck.OnWinCheckReady += HandleSetup;
        WinCheck.OnZonePoint += HandleScore;
    }

    private void OnDestroy()
    {
        //WinCheck.OnWinCheckReady -= HandleSetup;
        WinCheck.OnZonePoint -= HandleScore;
    }

    public void HandleSetup()
    {
        Debug.Log("handle setup");

        selectedGameMode = winCheck.selectedGameMode;

        widthChange = baseWidthOfTeam / winCheck.zonePointsToWin;

        zoneBackground.SetActive(selectedGameMode.HasZones);
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        //Debug.LogError("ui zone update");
        //UpdateTankProperties(targetPlayer, changedProps);
    }

    void HandleScore(Player player, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("zoneScore") && winCheck.selectedGameMode.HasZones)
        {
            //Debug.Log("ui zone score increase");
            //SetScoreBar();
        }
    }

    public void SetScoreBar()
    {
        Debug.LogWarning("set score bar");

        int yourScore = 0;
        int opposingScore = 0;

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; ++i)
        {
            if (PhotonNetwork.PlayerList[i].GetPhotonTeam() != PhotonNetwork.LocalPlayer.GetPhotonTeam())
            {
                Debug.Log($"{PhotonNetwork.PlayerList[i].NickName} is not on the same team");
                opposingTeamImage.color = teamInfo.teamColors[PhotonNetwork.PlayerList[i].GetPhotonTeam().Code - 1];
                if (PhotonNetwork.PlayerList[i].CustomProperties.ContainsKey("zoneScore"))
                {
                    opposingTeamText.text = PhotonNetwork.PlayerList[i].CustomProperties["zoneScore"].ToString();
                    opposingScore = (int)PhotonNetwork.PlayerList[i].CustomProperties["zoneScore"];
                    Debug.Log($"opposing score is {opposingScore}");
                }
                else
                {
                    opposingTeamText.text = "0";
                }
                break;
            }
        }

        yourTeamImage.color = teamInfo.teamColors[PhotonNetwork.LocalPlayer.GetPhotonTeam().Code - 1];
        
        yourTeamText.text = PhotonNetwork.LocalPlayer.CustomProperties["zoneScore"].ToString();
        yourScore = (int)PhotonNetwork.LocalPlayer.CustomProperties["zoneScore"];

        int dif = yourScore - opposingScore;

        int newWidth = baseWidthOfTeam + (dif * widthChange);
        yourTeamImage.rectTransform.sizeDelta = new Vector2(newWidth, yourTeamImage.rectTransform.sizeDelta.y);

        if (newWidth == baseWidthOfBar)
        {
            opposingTeamScore.SetActive(false);
        }
        else if (newWidth == 0)
        {
            yourTeamScore.SetActive(false);
        }
        else
        {
            opposingTeamScore.SetActive(true);
            yourTeamScore.SetActive(true);
        }
        opposingTeamImage.rectTransform.sizeDelta = new Vector2(baseWidthOfBar - newWidth, opposingTeamImage.rectTransform.sizeDelta.y);
    }
}
