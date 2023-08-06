using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using System;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

public class RoundManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private int inBetweenTime;
    [SerializeField] private GameObject roundScreen;
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private TMP_Text roundTimeText;

    [SerializeField] private TMP_Text scoreLineText;

    public static Action OnGameStarted = delegate { };

    private float roundTimer;

    private int roundNumber;
    private int numberOfRounds;

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
    }

    private void HandleStartGame()
    {
        roundScreen.SetActive(true);
        roundTimer = inBetweenTime;
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
                scoreLineText.SetText(changedProps["teamRoundScore"].ToString());
            }
        }
    }

    private void UpdateRoundNumber(int number)
    {
        roundText.SetText($"Round {number}");
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
                    PhotonNetwork.PlayerList[i].CustomProperties["teamRoundScore"] = 0;
                }
                PhotonNetwork.PlayerList[i].SetScore(1);
                PhotonNetwork.PlayerList[i].CustomProperties["teamRoundScore"] = 1;
            }
        }

        roundScreen.SetActive(true);
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "roundNumber", 2 } });
    }
}
