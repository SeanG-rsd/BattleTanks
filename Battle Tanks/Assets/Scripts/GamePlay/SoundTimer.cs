using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SoundTimer : MonoBehaviour
{
    [SerializeField] private float timeAlive;

    // Update is called once per frame
    void Update()
    {
        if (timeAlive > 0)
        {
            timeAlive -= Time.deltaTime;
        }
        else
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
