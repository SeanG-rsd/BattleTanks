using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class PhotonGameModeController : MonoBehaviourPunCallbacks
{
    private GameMode currentSelectedGameMode;
    [SerializeField] Button[] gameModes;

    //public static Action<GameMode> OnGameModeSelected = delegate { };

    private void Awake()
    {
        currentSelectedGameMode = null;
        UIGameMode.OnGameModeChanged += HandleGameModeChanged;
    }

    private void OnDestroy()
    {
        UIGameMode.OnGameModeChanged -= HandleGameModeChanged;
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

    public void ChooseGameMode()
    {
        Hashtable setGameMode = new Hashtable() { { "GAMEMODE", currentSelectedGameMode.Name} };
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "GAMEMODE", currentSelectedGameMode.Name.ToString() } });
    }
}
