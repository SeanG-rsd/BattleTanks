using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Pun.UtilityScripts;
using System;
using UnityEngine.UI;

public class TankShoot : MonoBehaviour
{

    public GameObject[] shootPoints;
    private int barrelIndex = 0;

    public float bulletSpeed;
    public float bulletSize;

    bool ableToShoot;
    public float shootDelay;
    float shooting;

    bool reload;
    public float reloadTime;
    float reloading;

    public int maxAmmo = 15;
    [SerializeField] private float currentAmmo;

    public int bulletDamage;

    Tank tank;
    TeamInfo teamInfo;

    PhotonView view;

    private bool shoot = false;

    [SerializeField] GameObject[] bulletPrefabs;

    [SerializeField] private RectTransform ammoMask;
    private float originalSize;

    // Start is called before the first frame update
    void Start()
    {
        teamInfo = FindObjectOfType<TeamInfo>();
        view = GetComponent<PhotonView>();
        tank = GetComponent<Tank>();
        if (view.IsMine)
        {
            currentAmmo = maxAmmo;
        }

        originalSize = ammoMask.rect.width;
        
    }

    private void Awake()
    {
        TapToShoot.TapShoot += HandleShoot;
        TapToShoot.TapToReload += HandleReload;
        TapToShoot.StopReload += HandleStopReload;
    }

    private void OnDestroy()
    {
        TapToShoot.TapShoot -= HandleShoot;
        TapToShoot.TapToReload -= HandleReload;
        TapToShoot.StopReload -= HandleStopReload;
    }

    private void HandleShoot()
    {
        shoot = true;
    }

    private void HandleReload()
    {
        reload = true;
    }

    private void HandleStopReload()
    {
        reload = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (view.IsMine)
        {

            if (shoot)
            {
                shoot = false;
                if ((int)currentAmmo > 0 && ableToShoot && !reload)
                {
                    Debug.Log("shot");
                    Shoot();
                    currentAmmo -= 1;
                    SetAmmoBar();
                    ableToShoot = false;
                    shooting = shootDelay;

                    barrelIndex++;
                    if (barrelIndex > shootPoints.Length - 1)
                    {
                        barrelIndex = 0;
                    }
                }
            }

            if (reload)
            {
                Debug.Log("reloading");
                currentAmmo += (Time.deltaTime / reloadTime);
                Mathf.Clamp(currentAmmo, 0, maxAmmo);
                SetAmmoBar();
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

    private void SetAmmoBar()
    {
        float value = currentAmmo / maxAmmo;
        ammoMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize * value);
    }

    void Shoot()
    {

        GameObject bullet = PhotonNetwork.Instantiate(bulletPrefabs[tank.teamIndex - 1].name, shootPoints[barrelIndex].transform.position, Quaternion.identity);

        bullet.GetComponent<Bullet>().Shoot(bulletSpeed, bulletSize, shootPoints[barrelIndex].transform, bulletDamage);
        
    }
}
