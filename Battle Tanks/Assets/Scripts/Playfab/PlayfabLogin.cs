using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;

public class PlayfabLogin : MonoBehaviour
{
    [SerializeField] private string username;
    void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "9F430";
        }
    }

    public void SetUsername(string name)
    {
        username = name;
        PlayerPrefs.SetString("USERNAME", username);
    }

    public void LogIn()
    {
        if (!IsValidUsername())
        {
            return;
        }

        LoginWithCustomId();
    }

    private bool IsValidUsername()
    {
        bool valid = false;

        if (username.Length >= 3 && username.Length <= 24)
        {
            valid = true;
        }

        return valid;
    }

    private void LoginWithCustomId()
    {
        Debug.Log($"Login to Playfab as {username}");
        var request = new LoginWithCustomIDRequest { CustomId = username, CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginCustomIdSuccess, OnFailure);
    }

    private void UpdateDisplayName(string displayName)
    {
        Debug.Log($"Updating Playfab account display name to {displayName}");
        var request = new UpdateUserTitleDisplayNameRequest { DisplayName = displayName };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameSuccess, OnFailure);
    }

    private void OnLoginCustomIdSuccess(LoginResult result)
    {
        Debug.Log($"You have logged into PlayFab using custom id {username}");
        UpdateDisplayName(username);
    }

    private void OnDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log($"You have updated the displayname of the playfab account!");
        SceneController.LoadScene("MainMenu");
    }

    private void OnFailure(PlayFabError error)
    {
        Debug.Log($"There was an issue with your request: {error.GenerateErrorReport()}");
    }
}
