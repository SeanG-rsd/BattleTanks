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

    [SerializeField] private RectTransform healthBarContainer;
    [SerializeField] private GameObject heartPrefab;

    [SerializeField] private int maxHearts;
    [SerializeField] private int currentHearts;

    public bool hasRespawned = true;

    private bool gmDealsWithHearts = false;

    Tank thisTank;
    

    PhotonView view;
    ExitGames.Client.Photon.Hashtable playerPropeties = new ExitGames.Client.Photon.Hashtable();

    public static Action<Tank> OnDeath = delegate { };
    public static Action<Tank> OnOutOfHearts = delegate { };


    private void Awake()
    {
        thisTank = GetComponent<Tank>();

        Tank.OnAlive += HandleTankAlive;
        Tank.OnNewRound += HandleReset;
    }

    private void OnDestroy()
    {
        Tank.OnAlive -= HandleTankAlive;
        Tank.OnNewRound -= HandleReset;
    }

    void Start()
    {
        view = GetComponent<PhotonView>();
        if (view.IsMine)
        {
            gmDealsWithHearts = thisTank.selectedGameMode.HasHearts;
            healthbar.SetActive(true);
            healthBarContainer.parent.gameObject.SetActive(true);
            
            currentHealth = maxHealth;
            currentHearts = maxHearts;
            playerPropeties["currentHealth"] = currentHealth;
            PhotonNetwork.SetPlayerCustomProperties(playerPropeties);
            healthbarText.text = currentHealth.ToString();

            if (!gmDealsWithHearts)
            {
                healthBarContainer.parent.gameObject.SetActive(false);
            }
            else
            {
                ResetHeartBar();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (view.IsMine)
        {
            if (playerPropeties.ContainsKey("currentHealth"))
            {
                currentHealth = (int)playerPropeties["currentHealth"];
                healthbarText.text = currentHealth.ToString();
            }

            if (!Alive() && hasRespawned)
            {
                OnDeath?.Invoke(gameObject.GetComponent<Tank>());
                
                hasRespawned = false;

                if (gmDealsWithHearts)
                {
                    RemoveHeart();
                }
            }

            if (Input.GetKeyUp(KeyCode.K))
            {
                playerPropeties["currentHealth"] = 0;
            }
        }        
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        playerPropeties["currentHealth"] = currentHealth;
    }

    private void RemoveHeart()
    {
        if (currentHearts > 0)
        {
            Destroy(healthBarContainer.GetChild(0).gameObject);
            currentHearts--;
        }

        if (currentHearts <= 0)
        {
            OnOutOfHearts?.Invoke(gameObject.GetComponent<Tank>());
        }
    }

    private void ResetHeartBar()
    {
        for (int i = 0; i < healthBarContainer.transform.childCount; i++)
        {
            Destroy(healthBarContainer.GetChild(i).gameObject);
        }

        for (int i = 0; i < maxHearts; ++i)
        {
            GameObject heart = Instantiate(heartPrefab, healthBarContainer.position, Quaternion.identity);

            heart.transform.SetParent(healthBarContainer);

            heart.transform.localScale = Vector3.one;
        }

        currentHearts = maxHearts;
    }

    private void HandleReset(Tank tank)
    {
        Debug.Log("health reset handled");
        ResetHeartBar();
        ResetHealth();
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

    private void HandleTankAlive(Tank tank)
    {
        hasRespawned = true;
        ResetHealth();
    }
}
