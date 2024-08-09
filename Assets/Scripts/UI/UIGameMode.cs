using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameMode : MonoBehaviour
{

    [SerializeField] GameMode gameMode;

    public static Action<GameMode, Button> OnGameModeChanged = delegate { };
    // Start is called before the first frame updat

    public void OnClickChooseGameMode()
    {
        OnGameModeChanged?.Invoke(gameMode, GetComponent<Button>());
    }
}
