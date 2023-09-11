using ExitGames.Client.Photon;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using TMPro;
using System.Globalization;
using System.Reflection;

public class Tank : MonoBehaviourPunCallbacks
{
    public PhotonView view;

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

    public Transform flagHolder;
    public Flag myFlag;


    [SerializeField] private GameObject respawnTimerObject;
    [SerializeField] private TMP_Text respawnTimerText;

    [SerializeField] private GameObject tankCanvas;

    private List<Player> damageDealersBeforeDeath;

    GameObject mapGenerator;
    public GameMode selectedGameMode;

    public static Action<Tank> OnRespawn = delegate { };
    public static Action<Tank> OnAlive = delegate { };

    public static Action<Tank> OnStart = delegate { };
    public static Action<Tank> OnStarted = delegate { };

    public static Action<Tank> OnNewRound = delegate { };

    public static Action<Tank, Player> OnBeginGame = delegate { };

    public static Action<Player> OnMasterLeave = delegate { };

    private TankRespawnPoint myRespawn;
    private bool goHome;

    LeaveGame leaveGame;

    private void Awake()
    {
        mapGenerator = FindObjectOfType<MapGeneator>().gameObject;
        selectedGameMode = mapGenerator.GetComponent<MapGeneator>().selectedGameMode;
        damageDealersBeforeDeath = new List<Player>();

        TankHealth.OnDeath += HandleTankDeath;
        GameManager.OnStartGame += HandleStartGame;
        RoundManager.OnGameStarted += HandleGameStarted;
        RoundManager.OnRoundStarted += HandleNewRound;
        WinCheck.OnRoundWon += HandleRoundWon;
        LeaveGame.OnLeaveGame += HandleLeaveGame;

        view = GetComponent<PhotonView>();

        

        leaveGame = FindObjectOfType<LeaveGame>();

        if (view.IsMine)
        {
            OnBeginGame?.Invoke(this, PhotonNetwork.LocalPlayer);
            myRespawn = FindMyRespawn();

            goHome = true;

            Hashtable hash = new Hashtable() { { "TankViewID", view.ViewID } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    private void OnDestroy()
    {
        TankHealth.OnDeath -= HandleTankDeath;
        GameManager.OnStartGame -= HandleStartGame;
        RoundManager.OnGameStarted -= HandleGameStarted;
        RoundManager.OnRoundStarted -= HandleNewRound;
        WinCheck.OnRoundWon -= HandleRoundWon;
        LeaveGame.OnLeaveGame -= HandleLeaveGame;
    }

    void Start()
    {
        tankHealth = gameObject.GetComponent<TankHealth>();
    }

    private void HandleLeaveGame()
    {
        Debug.Log("quit application");
        PhotonNetwork.Destroy(gameObject);
    }

    private void Update()
    {
        if (respawning)
        {
            if (respawnTimer < 0)
            {
                damageDealersBeforeDeath.Clear();
                OnAlive?.Invoke(this);
                respawning = false;
                invicible = true;
                invincibleTimer = invincibleTime;
                respawnTimerObject.SetActive(false);
                nonHit = false;
                tankCanvas.SetActive(true);
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

        if (view.IsMine && goHome)
        {
            transform.position = GoHome();
            goHome = false;
        }


    }

    private TankRespawnPoint FindMyRespawn()
    {
        TankRespawnPoint[] respawns = FindObjectsOfType<TankRespawnPoint>();

        foreach (TankRespawnPoint p in respawns)
        {
            if (p.teamIndex == this.teamIndex)
            {
                return p;
            }
        }

        return null;
    }

    private Vector3 GoHome()
    {
        Debug.Log("go home");

        Vector2 pos = myRespawn.GetPoint();

        Vector3 position = new Vector3(pos.x, transform.position.y, pos.y);
        return position;
    }

    private void HandleTankDeath(Tank tank)
    {
        if (tank == this && view.IsMine)
        {
            if (myFlag != null)
            {
                if (myFlag.isHeld && myFlag.thisTank == this)
                {
                    myFlag.GoHome();
                }
            }

            OnRespawn?.Invoke(this);
            respawning = true;
            respawnTimer = respawnTime;
            Debug.Log("tank has died");
            respawnTimerObject.SetActive(true);
            respawnTimerText.text = Mathf.RoundToInt(respawnTimer).ToString();
            tankCanvas.SetActive(false);

            HandleDamageDealers();

            int numDeaths = 0;

            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("DEATHS"))
            {
                numDeaths = (int)PhotonNetwork.LocalPlayer.CustomProperties["DEATHS"] + 1;
            }
            else
            {
                numDeaths = 1;
            }

            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "DEATHS", numDeaths } });
        }
    }

    private void HandleDamageDealers()
    {
        if (damageDealersBeforeDeath.Count == 0) return;

        for (int i = 0; i < damageDealersBeforeDeath.Count - 1; ++i)
        {
            if (damageDealersBeforeDeath[i] != damageDealersBeforeDeath[damageDealersBeforeDeath.Count - 1])
            {
                int numAssists = 0;

                if (damageDealersBeforeDeath[i].CustomProperties.ContainsKey("ASSISTS"))
                {
                    numAssists = (int)damageDealersBeforeDeath[i].CustomProperties["ASSISTS"] + 1;
                }
                else
                {
                    numAssists = 1;
                }

                damageDealersBeforeDeath[i].SetCustomProperties(new Hashtable() { { "ASSISTS", numAssists } });
            }
        }

        int numKills = 0;

        if (damageDealersBeforeDeath[damageDealersBeforeDeath.Count - 1].CustomProperties.ContainsKey("KILLS"))
        {
            numKills = (int)damageDealersBeforeDeath[damageDealersBeforeDeath.Count - 1].CustomProperties["KILLS"] + 1;
        }
        else
        {
            numKills = 1;
        }

        damageDealersBeforeDeath[damageDealersBeforeDeath.Count - 1].SetCustomProperties(new Hashtable() { { "KILLS", numKills } });
    }

    private void Respawn()
    {
        if (view.IsMine)
        {
            Debug.Log("tank respawned");
            OnRespawn?.Invoke(this);
            OnAlive?.Invoke(this);
            invicible = false;
            respawning = false;
            nonHit = false;
            tankCanvas.SetActive(false);

            OnStart?.Invoke(this);
        }
    }

    private void HandleStartGame()
    {
        Debug.Log($"begin game for {PhotonNetwork.LocalPlayer.NickName}");
        Respawn();
    }

    private void HandleGameStarted()
    {
        if (view.IsMine)
        {
            OnStarted?.Invoke(this);
            tankCanvas.SetActive(true);
            respawnTimerObject.SetActive(false);
        }
    }

    private void HandleRoundWon(PhotonTeam team)
    {
        respawning = false;
        Debug.Log("handle round won for tank");
        tankCanvas.SetActive(false);
        OnNewRound?.Invoke(this);
        respawnTimerObject.SetActive(false);
    }

    private void HandleNewRound()
    {
        Hashtable hashtable = new Hashtable() { { "aliveState", 1 } };
        Debug.Log("set custom prop");

        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);

        Respawn();
        tankCanvas.SetActive(false);
        respawnTimerObject.SetActive(false);
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

                if (collision.gameObject.GetComponent<PhotonView>() != null)
                {
                    Debug.Log($"{PhotonNetwork.LocalPlayer.NickName} was shot by {collision.gameObject.GetComponent<PhotonView>().Owner}");
                    damageDealersBeforeDeath.Add(collision.gameObject.GetComponent<PhotonView>().Owner);
                }
            }
        }
    }
}
