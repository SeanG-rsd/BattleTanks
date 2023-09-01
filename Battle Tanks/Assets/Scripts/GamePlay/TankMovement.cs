using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

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

        Tank.OnRespawn += HandleTankDeath;
        Tank.OnAlive += HandleTankAlive;
        Tank.OnBeginGame += HandleStart;
        Tank.OnStarted += HandleRoundStarted;
    }

    private void OnDestroy()
    {
        Tank.OnRespawn -= HandleTankDeath;
        Tank.OnAlive -= HandleTankAlive;
        Tank.OnBeginGame += HandleStart;
        Tank.OnStarted += HandleRoundStarted;
    }

    // Update is called once per frame
    void Update()
    {
        if (view.IsMine && canMove)
        {
            Vector3 input = new Vector3(0, 0, Input.GetAxisRaw("Vertical"));

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

    private void HandleStart(Tank tank, Player player)
    {
        Debug.Log("handle start");
        if (view.IsMine)
        {
            canMove = false;
            PlayerCamera.SetActive(false);
        }
        Debug.Log($"Can move is {canMove}");
    }

    private void HandleRoundStarted(Tank tank)
    {
        Debug.Log("round has started?");
        if (view.IsMine)
        {
            canMove = true;
            PlayerCamera.SetActive(true);
        }
    }
}
