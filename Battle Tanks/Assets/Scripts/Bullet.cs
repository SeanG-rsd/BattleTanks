using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    float bulletSpeed;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot(float speed, float size, Transform origin)
    {
        rb = GetComponent<Rigidbody>();
        transform.localScale = size * transform.localScale;
        rb.AddForce(origin.forward * speed);
    }
}
