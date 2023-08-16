using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class PhotonGameSettingsController : MonoBehaviourPunCallbacks
{
    private GameMode currentSelectedGameMode;
    [SerializeField] Button[] gameModes;

    private int currentSelectedRoundNumber;
    [SerializeField] Button[] roundNumbers;

    private void Awake()
    {
        currentSelectedGameMode = null;
        UIGameMode.OnGameModeChanged += HandleGameModeChanged;
        UIRoundController.OnNumberOFRoundsChanged += HandleRoundNumChanged;
    }

    private void OnDestroy()
    {
        UIGameMode.OnGameModeChanged -= HandleGameModeChanged;
        UIRoundController.OnNumberOFRoundsChanged -= HandleRoundNumChanged;
    }

    private void HandleGameModeChanged(GameMode gameMode, Button button)
    {
        currentSelectedGameMode = gameMode;
        foreach (Button b in gameModes)
        {
            b.interactable = true;
        }
        button.interactable = false;
    }

    private void HandleRoundNumChanged(int roundNumber, Button button)
    {
        currentSelectedRoundNumber = roundNumber;
        foreach (Button b in roundNumbers)
        {
            b.interactable = true;
        }
        button.interactable = false;
    }

    public void ChooseGameMode()
    {
        if (currentSelectedGameMode != null && currentSelectedRoundNumber != 0)
        {
            Hashtable setGameMode = new Hashtable() { { "GAMEMODE", currentSelectedGameMode.Name }, { "NUMBEROFROUNDS", currentSelectedRoundNumber } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "GAMEMODE", currentSelectedGameMode.Name }, { "NUMBEROFROUNDS", currentSelectedRoundNumber } });
        }
    }
}
