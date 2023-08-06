using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TankMovement : MonoBehaviour
{
    PhotonView view;

    public float speed;

    public float bodyRotateSpeed;
    public GameObject PlayerCamera;

    public Rigidbody rb;

    private bool canMove = true;

    private Tank tank;

    private void Awake()
    {
        view = gameObject.GetComponent<PhotonView>();
        if (view.IsMine)
        {
            PlayerCamera.SetActive(true);
        }

        Tank.OnRespawn += HandleTankDeath;
        Tank.OnAlive += HandleTankAlive;
    }

    private void OnDestroy()
    {
        Tank.OnRespawn -= HandleTankDeath;
        Tank.OnAlive -= HandleTankAlive;
    }

    // Update is called once per frame
    void Update()
    {
        if (view.IsMine && canMove)
        {
            Vector3 input = new Vector3(0, 0, Input.GetAxisRaw("Vertical"));

            //transform.position += input.normalized * Time.deltaTime * speed;

            transform.position += transform.forward * Input.GetAxisRaw("Vertical") * speed * Time.deltaTime;

            transform.Rotate(0, bodyRotateSpeed * Time.deltaTime * Input.GetAxisRaw("Horizontal"), 0, Space.World);
        }
    }

    private void HandleTankDeath(Tank tank)
    {
        if (view.IsMine)
        {
            canMove = false;
            PlayerCamera.SetActive(false);
        }
    }

    private void HandleTankAlive(Tank tank)
    {
        if (view.IsMine)
        {
            canMove = true;
            PlayerCamera.SetActive(true);
        }
    }
}
