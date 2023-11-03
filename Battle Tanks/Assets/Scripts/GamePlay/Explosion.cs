using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float surviveTime;

    // Update is called once per frame
    void Update()
    {
        surviveTime -= Time.deltaTime;
        if (surviveTime < 0)
        {
            GetComponent<PhotonView>().RPC("OnDestroy", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]

    private void OnDestroy()
    {
        Destroy(gameObject);
    }
}
