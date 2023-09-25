using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.InputSystem;

public class TankMovement : MonoBehaviour
{
    PhotonView view;

    public float speed;

    public float bodyRotateSpeed;
    public GameObject PlayerCamera;

    public Rigidbody rb;

    private bool canMove = true;

    private Tank tank;

    [SerializeField] PlayerInput tankInput;

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
        if (canMove && view.IsMine)
        {
            float input = tankInput.actions["Move"].ReadValue<Vector2>().y;
            
            transform.position += transform.forward * input * speed * Time.deltaTime;

            transform.Rotate(0, tankInput.actions["Move"].ReadValue<Vector2>().x * bodyRotateSpeed * Time.deltaTime, 0, Space.World);
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
