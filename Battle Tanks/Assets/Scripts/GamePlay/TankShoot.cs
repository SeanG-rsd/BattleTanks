using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Pun.UtilityScripts;
using System;

public class TankShoot : MonoBehaviour
{

    public GameObject[] shootPoints;
    public int barrelIndex;

    public float bulletSpeed;
    public float bulletSize;

    bool ableToShoot;
    public float shootDelay;
    float shooting;

    bool reload;
    public float reloadTime;
    float reloading;

    public int maxAmmo = 15;
    int currentAmmo;
    public TMP_Text ammoDisplayText;
    public TMP_Text maxAmmoDisplayText;
    public GameObject ammoDisplay;

    public int bulletDamage;

    Tank tank;
    TeamInfo teamInfo;

    PhotonView view;

    private bool shoot = false;

    [SerializeField] GameObject[] bulletPrefabs;

    // Start is called before the first frame update
    void Start()
    {
        teamInfo = FindObjectOfType<TeamInfo>();
        view = GetComponent<PhotonView>();
        tank = GetComponent<Tank>();
        if (view.IsMine)
        {
            currentAmmo = maxAmmo;
            maxAmmoDisplayText.text = maxAmmo.ToString();
            ammoDisplayText.text = currentAmmo.ToString();
            ammoDisplay.SetActive(true);
        }
        
    }

    private void Awake()
    {
        TapToShoot.TapShoot += HandleShoot;
        TapToShoot.TapToReload += HandleReload;
    }

    private void OnDestroy()
    {
        TapToShoot.TapShoot -= HandleShoot;
        TapToShoot.TapToReload -= HandleReload;
    }

    private void HandleShoot()
    {
        shoot = true;
    }

    private void HandleReload()
    {
        reload = true;
        reloading = reloadTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (view.IsMine)
        {

            if (shoot)
            {
                shoot = false;
                if (currentAmmo > 0 && ableToShoot && !reload)
                {
                    Debug.Log("shot");
                    Shoot();
                    currentAmmo--;
                    ableToShoot = false;
                    shooting = shootDelay;
                    ammoDisplayText.text = currentAmmo.ToString();
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                reload = true;
                reloading = reloadTime;
            }

            if (reload)
            {
                reloading -= Time.deltaTime;

                if (reloading < 0)
                {
                    reload = false;
                    currentAmmo = maxAmmo;
                    ammoDisplayText.text = currentAmmo.ToString();
                }
            }

            if (!ableToShoot)
            {
                shooting -= Time.deltaTime;

                if (shooting < 0)
                {
                    ableToShoot = true;
                }
            }
        }
    }

    void Shoot()
    {

        GameObject bullet = PhotonNetwork.Instantiate(bulletPrefabs[tank.teamIndex - 1].name, shootPoints[barrelIndex].transform.position, Quaternion.identity);

        bullet.GetComponent<Bullet>().Shoot(bulletSpeed, bulletSize, shootPoints[barrelIndex].transform, bulletDamage);
        
    }
}
