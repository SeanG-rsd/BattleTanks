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


    // Start is called before the first frame update
    void Start()
    {
        view = gameObject.GetComponent<PhotonView>();
        if (view.IsMine)
        {
            PlayerCamera.SetActive(true);
        }
    }

    private void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (view.IsMine)
        {
            Vector3 input = new Vector3(0, 0, Input.GetAxisRaw("Vertical"));

            //transform.position += input.normalized * Time.deltaTime * speed;

            transform.position += transform.forward * Input.GetAxisRaw("Vertical") * speed * Time.deltaTime;

            transform.Rotate(0, bodyRotateSpeed * Time.deltaTime * Input.GetAxisRaw("Horizontal"), 0, Space.World);
        }
    }
}
