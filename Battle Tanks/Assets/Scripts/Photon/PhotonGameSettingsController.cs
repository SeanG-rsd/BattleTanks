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
    [SerializeField] private GameObject gameModeSelected;

    private int currentSelectedRoundNumber;
    [SerializeField] Button[] roundNumbers;
    [SerializeField] private GameObject roundNumberSelected;

    [SerializeField] private GameObject continueButton;
    

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

    private void Update()
    {
        if (currentSelectedGameMode != null && currentSelectedRoundNumber != 0)
        {
            continueButton.SetActive(true);
        }
    }

    private void HandleGameModeChanged(GameMode gameMode, Button button)
    {
        gameModeSelected.SetActive(true);
        currentSelectedGameMode = gameMode;
        gameModeSelected.transform.position = button.transform.position;
    }

    private void HandleRoundNumChanged(int roundNumber, Button button)
    {
        roundNumberSelected.SetActive(true);
        currentSelectedRoundNumber = roundNumber;
        roundNumberSelected.transform.position = button.transform.position;
    }

    public void ChooseGameMode()
    {
        if (currentSelectedGameMode != null && currentSelectedRoundNumber != 0)
        {
            Hashtable setGameMode = new Hashtable() { { "GAMEMODE", currentSelectedGameMode.Name }, { "NUMBEROFROUNDS", currentSelectedRoundNumber } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "GAMEMODE", currentSelectedGameMode.Name }, { "NUMBEROFROUNDS", currentSelectedRoundNumber }, { "GAMEMODESELECTED", true } });
        }
    }
}
