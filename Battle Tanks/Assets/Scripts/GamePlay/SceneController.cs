using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class SceneController : MonoBehaviour
{

    private void Awake()
    {
        PhotonRoomController.OnStartGame += HandleStartGame;
    }

    private void OnDestroy()
    {
        PhotonRoomController.OnStartGame -= HandleStartGame;
    }

    private void HandleStartGame()
    {
        LoadScene("Game");
    }

    public static void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
