using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapToShoot : MonoBehaviour
{
    public static Action TapShoot = delegate { };
    public static Action TapToReload = delegate { };

    [SerializeField] PhotonView view;

    public void Shoot()
    {
        TapShoot?.Invoke();
        Debug.Log("shoot");
    }

    public void Reload()
    {
        TapToReload?.Invoke();
        Debug.Log("shoot");
    }
}
