using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Realtime;
using Photon.Pun;

public class Bullet : MonoBehaviour
{
    float bulletSpeed;

    Rigidbody rb;

    public int teamIndex = -1;
    public int damage = 0;

    public Player parentPlayer;

    private float noDieTime = 0.1f;
    private bool noDie = true;

    [SerializeField] private GameObject explosionPrefab;
    private GameObject parent;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (noDie)
        { 
            noDieTime -= Time.deltaTime;
            if (noDieTime < 0)
            {
                noDie = false;
            }
        }
    }

    public void SpawnExplosion(bool follow)
    {
        GameObject explosion = PhotonNetwork.Instantiate(explosionPrefab.name, transform.position, Quaternion.identity);
        explosion.GetComponent<Explosion>().Follow(parent, follow);
    }

    public void Shoot(float speed, float size, Transform origin, int dam, GameObject gm)
    {
        rb = GetComponent<Rigidbody>();
        transform.localScale = size * transform.localScale;
        rb.AddForce(origin.forward * speed);
        damage = dam;
        parent = gm;

        SpawnExplosion(true);
    }

    public void Destroy()
    {
        if (!noDie && GetComponent<PhotonView>().IsMine)
        {
            SpawnExplosion(false);
            GetComponent<PhotonView>().RPC("OnDestroy", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]

    private void OnDestroy()
    {
        Destroy(gameObject);
    }
}
