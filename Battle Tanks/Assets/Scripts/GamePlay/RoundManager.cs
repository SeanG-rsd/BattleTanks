using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using System;

public class RoundManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private int inBetweenTime;
    [SerializeField] private GameObject roundScreen;
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private TMP_Text roundTimeText;

    public static Action OnGameStarted = delegate { };

    private float roundTimer;

    private int roundNumber;

    private void Awake()
    {
        GameManager.OnStartGame += HandleStartGame;
        GameManager.OnGenerateMap += HandleGenerateMap;
    }

    private void OnDestroy()
    {
        GameManager.OnStartGame -= HandleStartGame;
        GameManager.OnGenerateMap -= HandleGenerateMap;
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

    private void UpdateRoundNumber(int number)
    {
        roundText.SetText($"Round {number}");
    }
}
