using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankCarosel : MonoBehaviour
{
    [SerializeField] private float speedRotation;

    [SerializeField] private float[] snapSpots;

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        float x = -Input.GetAxis("Mouse X");
#elif UNITY_ANDROID || UNITY_IOS

        float x = -Input.touches[0].deltaPosition.x;
#else
         float x = -Input.GetAxis("Mouse X");
#endif
        if (Input.GetMouseButton(0))
        {
            transform.rotation *= Quaternion.AngleAxis(x * speedRotation, Vector3.up);
        }


        if (Input.GetMouseButtonUp(0))
        {
            SnapToClosest();
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
