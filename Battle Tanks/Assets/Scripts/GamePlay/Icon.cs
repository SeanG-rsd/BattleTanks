using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class Icon : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float scale;
    [SerializeField] private int teamIndex;
    [SerializeField] private bool isTank;

    private string localPlayerTag;

    [SerializeField] private string SAFE_TAG;

    [SerializeField] private Image image;
    private string playerName = "";

    public static Action<GameObject> OnTankIconMade;

    
    private void Awake()
    {
        MiniMap miniMap = FindObjectOfType<MiniMap>();
        transform.SetParent(miniMap.miniMapContainer);

        localPlayerTag = "MiniMap" + PhotonNetwork.LocalPlayer.GetPhotonTeam().Name;

        if (PhotonNetwork.IsMasterClient)
        {
            transform.localScale = new Vector3(scale, scale, scale);
        }

        SetActive();
    }

    private void SetActive()
    {
        if (gameObject.tag != SAFE_TAG)
        {
            if (localPlayerTag != gameObject.tag)
            {
                image.color = new Color(0,0,0,0);
            }
            else if (isTank && localPlayerTag == gameObject.tag && gameObject.GetComponent<PhotonView>().Owner.IsMasterClient && playerName == "")
            {
                //gameObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
                OnTankIconMade?.Invoke(this.gameObject);
            }
        }
    }

    public void Test(string name)
    {
        playerName = name;
    }

    public void Destroy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GetComponent<PhotonView>().RPC("OnDestroy", RpcTarget.AllBuffered);
        }
    }

    public void SetOwner(string id)
    {
        GetComponent<PhotonView>().RPC("HasBeenChosen", RpcTarget.AllBuffered, id);
    }

    [PunRPC]

    private void OnDestroy()
    {
        Destroy(gameObject);
    }

    private void HasBeenChosen(string owner)
    {
        playerName = owner;
    }
}
