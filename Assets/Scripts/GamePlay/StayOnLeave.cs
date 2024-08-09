using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class StayOnLeave : MonoBehaviourPunCallbacks
{
    PhotonView view;
    void Start()
    {
        view = GetComponent<PhotonView>();
    }

    private void Awake()
    {
        Tank.OnMasterLeave += HandleTransferOwnership;
    }

    private void OnDestroy()
    {
        Tank.OnMasterLeave -= HandleTransferOwnership;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer == view.Owner)
        {
            view.TransferOwnership(PhotonNetwork.MasterClient);
        }
    }

    private void HandleTransferOwnership(Player currentOwner)
    {
        if (currentOwner == view.Owner)
        {
            view.TransferOwnership(PhotonNetwork.MasterClient);
        }
    }
}
