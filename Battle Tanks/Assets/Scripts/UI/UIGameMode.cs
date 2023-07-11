using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameMode : MonoBehaviour
{

    [SerializeField] GameMode gameMode;
    [SerializeField] TMP_Text gameModeText;

    public static Action<GameMode, Button> OnGameModeChanged = delegate { };
    // Start is called before the first frame update
    void Start()
    {
        gameModeText.SetText(gameMode.Name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickChooseGameMode()
    {
        OnGameModeChanged?.Invoke(gameMode, GetComponent<Button>());
    }
}
