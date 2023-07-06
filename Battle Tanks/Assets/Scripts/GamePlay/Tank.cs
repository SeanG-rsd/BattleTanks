using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using Photon.Realtime;

public class Tank : MonoBehaviourPunCallbacks
{
    TeamInfo teamInfo;
    PhotonView view;

    TankHealth tankHealth;

    public int teamIndex = -1;

    string teamName;

    

    ExitGames.Client.Photon.Hashtable playerPropeties = new ExitGames.Client.Photon.Hashtable();
    Player player;
    // Start is called before the first frame update
    void Start()
    {
        tankHealth = gameObject.GetComponent<TankHealth>();
        
        teamInfo = FindObjectOfType<TeamInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (player == targetPlayer)
        {
            UpdateTankProperties(targetPlayer);
        }
    }

    void UpdateTankProperties(Player player)
    {

        if (player.CustomProperties.ContainsKey("currentHealth"))
        {
            tankHealth.currentHealth = (int)player.CustomProperties["currentHealth"];
        }
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            if (collision.gameObject.GetComponent<Bullet>().teamIndex != teamIndex)
            {
                tankHealth.ChangeHealth(-collision.gameObject.GetComponent<Bullet>().damage);
            }
        }
    }
}
