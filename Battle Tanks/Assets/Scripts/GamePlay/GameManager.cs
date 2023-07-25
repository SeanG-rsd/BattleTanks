using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using System;

public class GameManager : MonoBehaviour
{
    
    public static Action<Vector2, GameMode> OnGenerateMap = delegate { };

    [SerializeField] private GameMode[] possibleGameModes;
    private GameMode selectedGameMode;

    private Vector2 mapSize;

    [SerializeField] private Vector2[] possibleMapSizes;
    private void Start()
    {
        SetMapSize();
        GetGameMode();

        if (PhotonNetwork.IsMasterClient)
        {
            OnGenerateMap?.Invoke(mapSize, selectedGameMode);
        }
        
    }

    private void SetMapSize()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount <= 4)
        {
            mapSize = possibleMapSizes[0];
        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount > 4 && PhotonNetwork.CurrentRoom.PlayerCount <= 6)
        {
            mapSize = possibleMapSizes[1];
        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount > 6)
        {
            mapSize = possibleMapSizes[2];
        }
    }

    private void GetGameMode()
    {
        for (int i = 0; i < possibleGameModes.Length; i++)
        {
            if ((string)PhotonNetwork.CurrentRoom.CustomProperties["GAMEMODE"] == possibleGameModes[i].Name)
            {
                selectedGameMode = possibleGameModes[i];
                return;
            }
        }
    }
}
