using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TankCarosel : MonoBehaviour
{
    [SerializeField] private float speedRotation;

    [SerializeField] private float[] snapSpots;

    [SerializeField] private GameObject[] options;

    [SerializeField] private RectTransform clickBoundary;

    Rect bounds;

    bool rotate = false;

    // Update is called once per frame

    private void Start()
    {
        Vector2 pos = new Vector2(clickBoundary.position.x, clickBoundary.position.y);
        //Debug.Log(pos);
        //bounds = new Rect(Screen.width + clickBoundary.transform.parent.position.x, Screen.height + clickBoundary.transform.position.y , clickBoundary.rect.width, clickBoundary.rect.height);
        bounds = new Rect(pos.x, pos.y, 400, 400);
    }
    void Update()
    {
#if UNITY_EDITOR
        float x = -Input.GetAxis("Mouse X");
#elif UNITY_ANDROID || UNITY_IOS

        float x = -Input.touches[0].deltaPosition.x;
#else
         float x = -Input.GetAxis("Mouse X");
#endif
        if (Input.GetMouseButton(0) && bounds.Contains(Input.mousePosition))
        {
            Debug.Log("rotate");
            rotate = true;
        }

        if (rotate)
        {
            foreach (GameObject go in options)
            {
                go.transform.RotateAround(transform.position, transform.up, x * speedRotation);
            }
        }


        if (Input.GetMouseButtonUp(0))
        {
            rotate = false;
            //SnapToClosest();

        }
    }

    private void SnapToClosest()
    {
        float current = transform.rotation.y;
        if (current < 0) { current = -current; }

        float snap = 360.0f;

        foreach (float f in snapSpots)
        {
            if (current - f < snap)
            {
                snap = f;
            }
        }

        transform.Rotate(0, current + snap, 0);
    }
}
