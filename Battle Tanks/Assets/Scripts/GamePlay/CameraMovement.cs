using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Photon.Pun;


public class CameraMovement : MonoBehaviour
{
    public GameObject player;

    Camera cam;

    float horizontal = 800.0f;

    public float zoomSpeed = 4.0f;

    public Vector3 maxPos;
    public Vector3 minPos;

    public Vector3 currentPos;

    public float weight = 0.0f;

    public bool cannotSee;
    public float lastWeight;



    // Start is called before the first frame update
    void Start()
    {

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //CheckCollisions();

        float s = Input.GetAxis("Mouse ScrollWheel");
        float h = horizontal * Input.GetAxis("Mouse X");

        if (Input.GetAxis("Mouse X") != 0)
        {
            //transform.RotateAround(player.transform.position, Vector3.up, h * Time.deltaTime);
            player.transform.Rotate(0, h * Time.deltaTime / 2, 0);
        }
        /*
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            //Debug.Log(s);

            weight = Mathf.Clamp(weight + (s * zoomSpeed), 0, 1);
            lastWeight = weight;
            //Debug.Log(weight);

            UpdatePos(weight);
        }*/

    }

    void CheckCollisions()
    {
        RaycastHit hit;


        Ray ray = cam.ScreenPointToRay(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        //Debug.Log(Input.mousePosition);



        if (Physics.Raycast(ray, out hit))
        {


            Debug.Log(hit.transform.gameObject.tag);
            if (hit.transform.gameObject.tag == "Player")
            {
                cannotSee = false;
                if (weight < lastWeight)
                {
                    weight = Mathf.Clamp(weight += 0.05f, 0, 1);
                    //UpdatePos(weight);
                }
                return;
            }
            if (hit.transform.gameObject.tag != "Player")
            {
                if (!cannotSee) { //lastWeight = weight;
								  }
                cannotSee = true;
                weight = Mathf.Clamp(weight -= 0.05f, 0, 1);
                Debug.Log("Not player");
                //UpdatePos(weight);
            }
        }
    }
}