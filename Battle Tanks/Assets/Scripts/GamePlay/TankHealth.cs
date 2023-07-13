using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Security.Cryptography;

public class TankHealth : MonoBehaviour
{

    public int maxHealth;
    public int currentHealth;

    public TMP_Text healthbarText;
    public GameObject healthbar;

    PhotonView view;
    ExitGames.Client.Photon.Hashtable playerPropeties = new ExitGames.Client.Photon.Hashtable();

    public static Action<Tank> OnDeath = delegate { };


    private void Awake()
    {
        Tank.OnRespawn += HandleTankRespawn;
    }

    private void OnDestroy()
    {
        Tank.OnRespawn -= HandleTankRespawn;
    }

    void Start()
    {
        view = GetComponent<PhotonView>();
        if (view.IsMine)
        {
            healthbar.SetActive(true);
            currentHealth = maxHealth;
            playerPropeties["currentHealth"] = currentHealth;
            PhotonNetwork.SetPlayerCustomProperties(playerPropeties);
            healthbarText.text = currentHealth.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (view.IsMine)
        {
            currentHealth = (int)playerPropeties["currentHealth"];
            healthbarText.text = currentHealth.ToString();

            if (!Alive())
            {
                OnDeath?.Invoke(gameObject.GetComponent<Tank>());
            }

        }

        
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        playerPropeties["currentHealth"] = currentHealth;
    }

    public void ChangeHealth(int value)
    {
        playerPropeties["currentHealth"] = Mathf.Clamp(currentHealth + value, 0, maxHealth);
        PhotonNetwork.SetPlayerCustomProperties(playerPropeties);
    }

    private bool Alive()
    {
        if (currentHealth <= 0)
        {
            return false;
        }

        return true;
    }

    private void HandleTankRespawn(Tank tank)
    {
        ResetHealth();
        Debug.LogWarning("reset health");
    }
}
