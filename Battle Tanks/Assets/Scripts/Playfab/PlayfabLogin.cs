using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayfabLogin : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text messageText;
    public InputField emailInput;
    public InputField passwordInput;
    public InputField usernameInput;

    public InputField emailInputUsername;
    public InputField passwordInputUsername;
    [SerializeField] private Toggle rememberToggleUsername;
    [SerializeField] private Image toggleImageUsername;

    private bool didRemember;
    private bool wantsToRemember;

    [SerializeField] private GameObject LoginScreen;
    [SerializeField] private GameObject UserNameScreen;
    [SerializeField] private Toggle rememberToggle;

    [SerializeField] private Sprite toggleOnSprite;
    [SerializeField] private Sprite toggleOffSprite;

    [SerializeField] private Image toggleImage;

    [SerializeField] private string username;

    private bool registered = false;

    private void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "9F430";
        }

        if (PlayerPrefs.HasKey("REMEMBERME"))
        {
            if ((int)PlayerPrefs.GetInt("REMEMBERME") == 1)
            {
                emailInput.text = PlayerPrefs.GetString("EMAIL");
                passwordInput.text = PlayerPrefs.GetString("PASSWORD");
                wantsToRemember = true;
                rememberToggle.isOn = true;
                rememberToggleUsername.isOn = true;
            }
        }

        toggleImage.sprite = (rememberToggle.isOn) ? toggleOnSprite : toggleOffSprite;
        toggleImageUsername.sprite = (rememberToggleUsername.isOn) ? toggleOnSprite : toggleOffSprite;
    }
    public void RememberMe(Toggle toggle)
    {
        wantsToRemember = toggle.isOn;
        rememberToggleUsername.isOn = toggle.isOn;
        rememberToggle.isOn = toggle.isOn;

        toggleImage.sprite = (toggle.isOn) ? toggleOnSprite : toggleOffSprite;
        toggleImageUsername.sprite = (toggle.isOn) ? toggleOnSprite : toggleOffSprite;
    }
    public void RegisterButton()
    {
        if (passwordInput.text.Length < 6)
        {
            messageText.text = "Your password is too short!";
            return;
        }

        var request = new RegisterPlayFabUserRequest { Email = emailInputUsername.text, Password = passwordInputUsername.text, RequireBothUsernameAndEmail = false };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);   
    }

    public void CreateUsername()
    {
        LoginScreen.SetActive(!LoginScreen.activeSelf);
        UserNameScreen.SetActive(!UserNameScreen.activeSelf);

        emailInputUsername.text = emailInput.text;
        passwordInputUsername.text = passwordInput.text;
        
    }

    private void LoginScreenOn()
    {
        LoginScreen.SetActive(true);
        UserNameScreen.SetActive(false);
    }

    public void FinishRegistering()
    {
        if (usernameInput.text.Length >= 3 && usernameInput.text.Length <= 24)
        {
            SetUsername(usernameInput.text);

            if (wantsToRemember)
            {
                PlayerPrefs.SetString("EMAIL", emailInputUsername.text);
                PlayerPrefs.SetString("PASSWORD", passwordInputUsername.text);
                PlayerPrefs.SetInt("REMEMBERME", 1);
            }

            UpdateDisplayName(username);
        }
        else
        {
            messageText.text = "Enter a valid username between 3 and 24 characters";
        }
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        registered = true;
        messageText.text = "Registered!";
        FinishRegistering();
        
    }

    void OnError(PlayFabError error)
    {
        messageText.text = error.ErrorMessage;
        Debug.Log(error.ErrorMessage);
        LoginScreenOn();
    }

    public void LoginButton()
    {
        var request = new LoginWithEmailAddressRequest { Email = emailInput.text, Password = passwordInput.text };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
        SetUsername(PlayerPrefs.GetString("USERNAME"));
    }

    void OnLoginSuccess(LoginResult result)
    {
        messageText.text = "Logged In!";
        RememberMe();

        var request = new GetAccountInfoRequest { Email = emailInput.text };
        PlayFabClientAPI.GetAccountInfo(request, OnGetUsernameResult, OnFailure);
    }

    public void ResetPasswordButton()
    {
        var request = new SendAccountRecoveryEmailRequest { Email = emailInput.text, TitleId = "9F430" };

        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, OnError);
    }

    void OnPasswordReset(SendAccountRecoveryEmailResult result)
    {
        messageText.text = "Password reset mail sent!";
    }

    public void SetUsername(string name)
    {
        username = name;
        PlayerPrefs.SetString("USERNAME", username);
    }

    private void RememberMe()
    {
        PlayerPrefs.SetString("EMAIL", emailInput.text);
        PlayerPrefs.SetString("PASSWORD", passwordInput.text);
        PlayerPrefs.SetInt("REMEMBERME", 1);
    }

    private void UpdateDisplayName(string displayName)
    {
        Debug.Log($"Updating Playfab account display name to {displayName}");
        SetUsername(displayName);
        var request = new UpdateUserTitleDisplayNameRequest { DisplayName = displayName };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameSuccess, OnFailure);
    }

    private void OnLoginCustomIdSuccess(LoginResult result)
    {
        Debug.Log($"You have logged into PlayFab using custom id {username}");
    }

    private void OnGetUsernameResult(GetAccountInfoResult result)
    {
        Debug.LogWarning($"worked: {result.AccountInfo.TitleInfo.DisplayName}");
        UpdateDisplayName(result.AccountInfo.TitleInfo.DisplayName);
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
