using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MapWall : MonoBehaviour
{
    public Vector2 position;

    public WallType.WallOrientation type;
    public void Destroy()
    {
        this.GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void DestroyObject()
    {
        Destroy(this.gameObject);
    }
}
