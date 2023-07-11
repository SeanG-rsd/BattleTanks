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
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot(float speed, float size, Transform origin, int dam)
    {
        rb = GetComponent<Rigidbody>();
        transform.localScale = size * transform.localScale;
        rb.AddForce(origin.forward * speed);
        damage = dam;
    }
}
