using ExitGames.Client.Photon;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using TMPro;

public class Tank : MonoBehaviourPunCallbacks
{
    PhotonView view;

    TankHealth tankHealth;

    public int teamIndex = -1;

    public TankRespawnPoint respawnPoint;

    Player player;

    [SerializeField] private float respawnTime;
    private float respawnTimer;
    private bool respawning;

    [SerializeField] private float invincibleTime;
    private float invincibleTimer;
    private bool invicible;

    [SerializeField] private float nonHitTime;
    private float nonHitTimer;
    private bool nonHit;

    private bool canMove;

    [SerializeField] private GameObject respawnTimerObject;
    [SerializeField] private TMP_Text respawnTimerText;

    public static Action<Tank> OnRespawn = delegate { };
    public static Action<Tank> OnAlive = delegate { };


    private void Awake()
    {
        TankHealth.OnDeath += HandleTankDeath;
        RoundManager.OnGameStarted += Respawn;

        view = GetComponent<PhotonView>();

    }

    private void OnDestroy()
    {
        TankHealth.OnDeath -= HandleTankDeath;
        RoundManager.OnGameStarted -= Respawn;
    }

    void Start()
    {
        tankHealth = gameObject.GetComponent<TankHealth>();
    }

    private void Update()
    {
        if (respawning)
        {
            if (respawnTimer < 0)
            {
                OnAlive?.Invoke(this);
                respawning = false;
                invicible = true;
                invincibleTimer = invincibleTime;
                respawnTimerObject.SetActive(false);
                nonHit = false;
            }

            respawnTimer -= Time.deltaTime;
            respawnTimerText.text = Mathf.RoundToInt(respawnTimer).ToString();
        }

        if (invicible)
        {
            if (invincibleTimer < 0)
            {
                invicible = false;
            }

            invincibleTimer -= Time.deltaTime;
        }

        if (nonHit)
        {
            if (nonHitTimer < 0)
            {
                nonHit = false;
            }

            nonHitTimer -= Time.deltaTime;
        }
    }

    private void HandleTankDeath(Tank tank)
    {
        if (tank == this)
        {
            OnRespawn?.Invoke(this);
            respawning = true;
            respawnTimer = respawnTime;
            Debug.Log("tank has died");
            respawnTimerObject.SetActive(true);
            respawnTimerText.text = Mathf.RoundToInt(respawnTimer).ToString();
            canMove = false;
        }
    }

    public void Respawn()
    {
        Debug.Log("tank respawned");
        OnRespawn?.Invoke(this);
        OnAlive?.Invoke(this);
        invicible = false;
        respawning = false;
        nonHit = false;
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
        if (collision.gameObject.tag == "Bullet" && photonView.IsMine)
        {
            if (collision.gameObject.GetComponent<Bullet>().teamIndex != teamIndex && !invicible && tankHealth.hasRespawned && !nonHit)
            {
                nonHit = true;
                nonHitTimer = nonHitTime;
                tankHealth.ChangeHealth(-collision.gameObject.GetComponent<Bullet>().damage);
            }
        }
    }

    public void Destroy()
    {
        Hashtable hashtable = new Hashtable() { { "aliveState", 0 } };
        Debug.Log("set custom prop");

        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);

        this.GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void DestroyObject()
    {
        Destroy(this.gameObject);

    }
}
