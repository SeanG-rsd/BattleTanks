using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class SelectScreen : MonoBehaviour
{
    public GameObject waitForPlayersScreen;
    public GameObject teamSelectScreen;

    public TMP_Text numPlayers;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (waitForPlayersScreen != null)
        {
            CheckForMinPlayers();
        }
    }

    void GenerateScreen()
    {

    }

    void CheckForMinPlayers()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.Destroy(waitForPlayersScreen);
        }
        numPlayers.text = PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
    }
}
