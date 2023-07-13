using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class Tank : MonoBehaviourPunCallbacks
{
    PhotonView view;

    TankHealth tankHealth;

    public int teamIndex = -1;

    public TankRespawnPoint respawnPoint;

    ExitGames.Client.Photon.Hashtable playerPropeties = new ExitGames.Client.Photon.Hashtable();
    Player player;

    public static Action<Tank> OnRespawn = delegate { };


    private void Awake()
    {
        TankHealth.OnDeath += HandleTankDeath;
    }

    private void OnDestroy()
    {
        TankHealth.OnDeath -= HandleTankDeath;
    }

    void Start()
    {
        tankHealth = gameObject.GetComponent<TankHealth>();
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandleTankDeath(Tank tank)
    {
        if (tank == this)
        {
            OnRespawn?.Invoke(this);
        }
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
