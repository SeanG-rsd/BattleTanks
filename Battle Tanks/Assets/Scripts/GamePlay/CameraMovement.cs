using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Photon.Pun;
using UnityEngine.InputSystem;


public class CameraMovement : MonoBehaviour
{
    public GameObject player;

    Camera cam;

    float horizontal = 200.0f;

    public float zoomSpeed = 4.0f;

    public Vector3 maxPos;
    public Vector3 minPos;

    public Vector3 currentPos;

    public float weight = 0.0f;

    public bool cannotSee;
    public float lastWeight;

    [SerializeField] PlayerInput tankInput;
    [SerializeField] private GameObject tankTop;

    private List<MapWall> hiddenObjects;

    // Start is called before the first frame update
    void Start()
    {
        hiddenObjects = new List<MapWall>();
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckCollisions();

        float h = horizontal * tankInput.actions["Look"].ReadValue<Vector2>().x;

        tankTop.transform.Rotate(0, h * Time.deltaTime, 0);
        player.transform.Rotate(0, h * Time.deltaTime, 0);


    }

    void CheckCollisions()
    {
        RaycastHit hit;


        Ray ray = new Ray(transform.position, transform.forward);



        if (Physics.Raycast(ray, out hit))
        {        
            if (hit.transform.gameObject.tag == "Player")
            {
                Debug.Log("hit player");
                foreach (MapWall go in hiddenObjects)
                {
                    go.See();
                }
                hiddenObjects.Clear();
            }
            else if (hit.transform.gameObject.GetComponent<MapWall>() != null)
            {
                Debug.Log("hit wall");
                hiddenObjects.Add(hit.transform.gameObject.GetComponent<MapWall>());
                hit.transform.gameObject.GetComponent<MapWall>().Hide();
            }
        }
    }
}
