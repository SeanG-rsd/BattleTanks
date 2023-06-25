using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class GameManager
{
    public GameObject PlayerPrefab;
    public GameObject GameCanvas;
    public GameObject SceneCam;

    public GameObject disconnectMenu;
    private bool Off = false;

    public bool start;

    // Start is called before the first frame update
    void Start()
    {
        GameObject player = PhotonNetwork.Instantiate(PlayerPrefab.name, new Vector3(0, 0.75f, 0), Quaternion.identity, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!start)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
