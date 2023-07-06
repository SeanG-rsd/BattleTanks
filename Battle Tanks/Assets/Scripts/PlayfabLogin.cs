using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayfabLogin : MonoBehaviour
{
    [SerializeField] private string username;
    void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSharedSettings.TitleId))
        {
            PlayFabSharedSettings.TitleId = "9F430";
        }
    }

    public void SetUsername(string name)
    {
        username = name;
        PlayerPrefs.SetString("USERNAME", username);
    }
}
