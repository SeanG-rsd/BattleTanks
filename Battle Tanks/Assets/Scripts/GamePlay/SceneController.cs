using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using Photon.Pun;

public class SceneController : MonoBehaviour
{

    private void Awake()
    {
        PhotonRoomController.OnStartGame += HandleStartGame;
        RoundManager.OnGameOver += ReturnToLobby;
    }

    private void OnDestroy()
    {
        PhotonRoomController.OnStartGame -= HandleStartGame;
        RoundManager.OnGameOver -= ReturnToLobby;
    }

    private void HandleStartGame()
    {
        LoadScene("Game");
    }

    public static void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    private void ReturnToLobby()
    {
        //PhotonNetwork.LeaveRoom();
        //PhotonNetwork.LeaveLobby();

        PhotonNetwork.LoadLevel("MainMenu");
    }
}
