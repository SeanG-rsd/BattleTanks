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

    [SerializeField] private GameObject infoBar;

    [SerializeField] private RectTransform heartBarContainer;
    [SerializeField] private GameObject heartPrefab;

    [SerializeField] private RectTransform healthPointContainer;
    [SerializeField] private GameObject healthPointPrefab;
    private int maxHealthPoints = 10;

    [SerializeField] private int maxHearts;
    [SerializeField] private int currentHearts;

    public bool hasRespawned = true;

    private bool gmDealsWithHearts = false;

    Tank thisTank;
    

    PhotonView view;
    ExitGames.Client.Photon.Hashtable playerPropeties = new ExitGames.Client.Photon.Hashtable();

    public static Action<Tank> OnDeath = delegate { };
    public static Action<Tank> OnOutOfHearts = delegate { };

    [SerializeField] private GameObject flamePrefab;


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

            infoBar.SetActive(true);
            heartBarContainer.gameObject.SetActive(true);

            ResetHealth();
            SetHealthBar();

            if (!gmDealsWithHearts)
            {
                heartBarContainer.gameObject.SetActive(false);
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
            }

            if (!Alive() && hasRespawned)
            {
                Vector3 pos = new Vector3(transform.position.x, 0, transform.position.z);
                PhotonNetwork.Instantiate(flamePrefab.name, pos, Quaternion.Euler(-90,0,0));
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
        PhotonNetwork.SetPlayerCustomProperties(playerPropeties);
    }

    private void SetHealthBar()
    {
        for (int i = 0; i < healthPointContainer.transform.childCount; i++)
        {
            Destroy(healthPointContainer.GetChild(i).gameObject);
        }

        for (int i = 0; i < calculateHealthPoints(); ++i)
        {
            GameObject healthPoint = Instantiate(healthPointPrefab, healthPointContainer.position, Quaternion.identity);

            healthPoint.transform.SetParent(healthPointContainer);

            healthPoint.transform.localScale = Vector3.one;
        }
    }

    public int calculateHealthPoints()
    {
        float percent = (float)currentHealth / (float)maxHealth;
        Debug.Log(percent);
        int output = (int)(percent * maxHealthPoints);
        Debug.Log(output);

        if (output == 0)
        {
            return 1;
        }
        else
        {
            return output;
        }
    }

    private void RemoveHeart()
    {
        if (currentHearts > 0)
        {
            Destroy(heartBarContainer.GetChild(0).gameObject);
            currentHearts--;
        }

        if (currentHearts <= 0)
        {
            OnOutOfHearts?.Invoke(gameObject.GetComponent<Tank>());
        }
    }

    private void ResetHeartBar()
    {
        for (int i = 0; i < heartBarContainer.transform.childCount; i++)
        {
            Destroy(heartBarContainer.GetChild(i).gameObject);
        }

        for (int i = 0; i < maxHearts; ++i)
        {
            GameObject heart = Instantiate(heartPrefab, heartBarContainer.position, Quaternion.identity);

            heart.transform.SetParent(heartBarContainer);

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
        SetHealthBar();
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
        SetHealthBar();
    }
}
