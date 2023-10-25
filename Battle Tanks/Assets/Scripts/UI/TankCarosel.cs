using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TankCarosel : MonoBehaviour
{
    [SerializeField] private float speedRotation;

    [SerializeField] private float[] snapSpots;
    [SerializeField] private Vector3[] snapPositions;

    [SerializeField] private List<int> lastRotations = new List<int>();

    [SerializeField] private GameObject[] options;

    [SerializeField] private RectTransform clickBoundary;

    Rect bounds;

    bool rotate = false;
    bool snapToClosest = false;

    Hashtable playerPropeties = new Hashtable();
    [SerializeField] TMP_Text playerAvatar;
    public string[] avatars;

    private const string playAv = "playerAvatar";

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
            //Debug.Log("rotate");
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
            snapToClosest = true;
            //SnapToClosest();

        }

        if (snapToClosest)
        {
            SnapToClosest();
            snapToClosest = false;
        }
    }

    private void SnapToClosest() // snap spots is relative to the y rotation of each tank
    {
        List<float> availableSpots = new List<float>(snapSpots);

        List<Vector3> takenPositions = new List<Vector3>();

        int tankNum = 0;
        foreach (GameObject go in options)
        {
            int closest = 0;

            for (int i = 0; i < availableSpots.Count; i++)
            {
                Vector2 pos = new Vector2(go.transform.localPosition.x, go.transform.localPosition.z);
                Vector2 snap = new Vector2(snapPositions[closest].x, snapPositions[closest].z);
                Vector2 next = new Vector2(snapPositions[i].x, snapPositions[i].z);
                if (Vector2.Distance(pos, next) < Vector2.Distance(pos, snap) && !takenPositions.Contains(snapPositions[i]))
                {
                    closest = i;
                }
            }

            go.transform.localPosition = snapPositions[closest];
            takenPositions.Add(snapPositions[closest]);

            lastRotations[tankNum] = closest;
            float rotate = snapSpots[closest] - go.transform.localRotation.eulerAngles.y + (120 * (closest - lastRotations[tankNum]));
            go.transform.Rotate(new Vector3(0f, rotate, 0f), Space.World);

            if (closest == 0)
            {
                Debug.Log(go.name);
            }

            tankNum++;
        }
    }

    private void SetTank()
    {

    }
}
