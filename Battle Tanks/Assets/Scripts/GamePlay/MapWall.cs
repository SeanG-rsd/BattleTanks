using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MapWall : MonoBehaviour
{
    public Vector2 position;

    public WallType.WallOrientation type;

    public bool isTouchingBorder;

    [SerializeField] private GameObject visualWall;
    public void Destroy()
    {
        this.GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.AllViaServer);
    }

    public void Hide()
    {
        visualWall.SetActive(false);
    }

    public void See()
    {
        visualWall.SetActive(true);
    }

    [PunRPC]
    public void DestroyObject()
    {
        Destroy(this.gameObject);
    }
}
