using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using PlayFabFriendInfo = PlayFab.ClientModels.FriendInfo;
using PhotonFriendInfo = Photon.Realtime.FriendInfo;
using System;
using System.Linq;

public class PhotonFriendController : MonoBehaviourPunCallbacks
{
    [SerializeField] private float refreshCooldown;
    [SerializeField] private float refreshCountdown;
    [SerializeField] private List<PlayFabFriendInfo> friendList;

    public static Action<List<PhotonFriendInfo>> OnDisplayFriends = delegate { };
    private void Awake()
    {
        friendList = new List<PlayFabFriendInfo>();
        PlayfabFriendController.OnFriendListUpdated += HandleFriendsUpdated;
    }

    private void OnDestroy()
    {
        PlayfabFriendController.OnFriendListUpdated -= HandleFriendsUpdated;
    }

    private void HandleFriendsUpdated(List<PlayFabFriendInfo> friends)
    {
        friendList = friends;
        FindPhotonFriends(friends);
    }

    private static void FindPhotonFriends(List<PlayFabFriendInfo> friends)
    {
        if (friends.Count != 0)
        {
            string[] friendDisplayNames = friends.Select(f => f.TitleDisplayName).ToArray();
            PhotonNetwork.FindFriends(friendDisplayNames);
        }
        else
        {
            List<PhotonFriendInfo> friendList = new List<PhotonFriendInfo>();
            OnDisplayFriends?.Invoke(friendList);
        }
    }

    public override void OnFriendListUpdate(List<PhotonFriendInfo> friends)
    {
        OnDisplayFriends?.Invoke(friends);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (refreshCountdown > 0)
        {
            refreshCountdown -= Time.deltaTime;
        }
        else
        {
            refreshCountdown = refreshCooldown;
            if (PhotonNetwork.InRoom)
            {
                return;
            }
            FindPhotonFriends(friendList);
        }
    }
}
