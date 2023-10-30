using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TapToShoot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static Action TapShoot = delegate { };
    public static Action TapToReload = delegate { };
    public static Action StopReload = delegate { };

    [SerializeField] PhotonView view;

    [SerializeField] private Button button;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(eventData.pointerEnter);
        if (button.gameObject == eventData.pointerEnter)
        {
            Reload();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        NotReload();
    }
    public void Shoot()
    {
        TapShoot?.Invoke();
        Debug.Log("shoot");
    }

    public void Reload()
    {
        TapToReload?.Invoke();
    }

    public void NotReload()
    {
        StopReload?.Invoke();
    }
}
