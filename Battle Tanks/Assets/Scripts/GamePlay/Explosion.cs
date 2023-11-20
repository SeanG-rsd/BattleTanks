using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using PlayFab.ClientModels;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float surviveTime;
    [SerializeField] private GameObject myParent;

    private bool follow = false;

    public void Follow(GameObject parent, bool follow)
    {
        this.follow = follow;
        this.myParent = parent;
    }

    // Update is called once per frame
    void Update()
    {
        surviveTime -= Time.deltaTime;
        if (surviveTime < 0)
        {
            GetComponent<PhotonView>().RPC("OnDestroy", RpcTarget.AllBuffered);
        }

        if (GetComponent<PhotonView>().IsMine && follow)
        {
            transform.position = myParent.transform.position;
        }
    }

    [PunRPC]

    private void OnDestroy()
    {
        Destroy(gameObject);
    }
}
