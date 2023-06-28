using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TankShoot : MonoBehaviour
{
    public GameObject bulletPrefab;

    public GameObject[] shootPoints;
    public int barrelIndex;

    public float bulletSpeed;
    public float bulletSize;

    public float shootDelay;
    public float reloadSpeed;

    public int maxAmmo;
    int currentAmmo;

    PhotonView view;

    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && view.IsMine)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, shootPoints[barrelIndex].transform.position, Quaternion.identity);

        bullet.GetComponent<Bullet>().Shoot(bulletSpeed, bulletSize, shootPoints[barrelIndex].transform);
    }
}
