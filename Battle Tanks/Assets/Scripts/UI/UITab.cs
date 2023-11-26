using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITab : MonoBehaviour
{
    public bool isOpen = false;

    [SerializeField] private Vector3 openPosition;
    [SerializeField] private Vector3 closePosition;

    [SerializeField] private float speed;

    public void OpenTab()
    {
        transform.LeanMoveLocal(openPosition, speed);
        isOpen = true;
    }

    public void CloseTab()
    {
        transform.LeanMoveLocal(closePosition, speed);
        isOpen = false;
    }
}
