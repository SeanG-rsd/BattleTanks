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

    private string localPlayerTag;

    [SerializeField] private string SAFE_TAG;

    [SerializeField] private Image image;

    public static Action<GameObject> OnTankIconMade;
    public static Action<Sprite, Color, GameObject> OnMakeLocalIndicator;

    public enum IconType
    {
        Tower,
        Tank,
        Flag,
        Zone,
        Wall
    }

    public IconType iconType;

    
    private void Awake()
    {
        MiniMap miniMap = FindObjectOfType<MiniMap>();
        transform.SetParent(miniMap.miniMapContainer);

        localPlayerTag = "MiniMap" + PhotonNetwork.LocalPlayer.GetPhotonTeam().Name;

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
            else
            {
                if (iconType == IconType.Tank && PhotonNetwork.LocalPlayer == (Player)GetComponent<PhotonView>().InstantiationData[0])
                {
                    OnTankIconMade?.Invoke(gameObject);
                }
                else if (iconType == IconType.Tank)
                {
                   // OnMakeLocalIndicator?.Invoke(GetComponent<Image>().sprite, GetComponent<Image>().color, gameObject);
                }
            }
        }
        else
        {
            //OnMakeLocalIndicator?.Invoke(GetComponent<Image>().sprite, GetComponent<Image>().color, gameObject);
        }
    }

    public void Destroy()
    {
        if (PhotonNetwork.IsMasterClient)
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
