using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System;
using System.Linq;

public class PlayfabFriendController : MonoBehaviour
{

    public static Action<List<FriendInfo>> OnFriendListUpdated = delegate { };
    private List<FriendInfo> friends;
    private void Awake()
    {
        friends = new List<FriendInfo>();
        PhotonConnector.GetPhotonFriends += HandleGetFriends;
        UIAddFriend.OnAddFriend += HandleAddPlayfabFriend;
        UIFriend.OnRemoveFriend += HandleRemoveFriend;
    }

    private void OnDestroy()
    {
        PhotonConnector.GetPhotonFriends -= HandleGetFriends;
        UIAddFriend.OnAddFriend -= HandleAddPlayfabFriend;
        UIFriend.OnRemoveFriend -= HandleRemoveFriend;
    }

    private void HandleGetFriends()
    {
        GetPlayfabFriends();
    }

    private void HandleAddPlayfabFriend(string name)
    {
        var request = new AddFriendRequest { FriendTitleDisplayName = name };
        PlayFabClientAPI.AddFriend(request, OnFriendAddedSuccess, OnFailure);
    }

    private void HandleRemoveFriend(string name)
    {
        string id = friends.FirstOrDefault(f => f.TitleDisplayName == name).FriendPlayFabId;
        var request = new RemoveFriendRequest { FriendPlayFabId = id };
        PlayFabClientAPI.RemoveFriend(request, OnFriendRemoveSuccess, OnFailure);
    }

    private void OnFriendRemoveSuccess(RemoveFriendResult result)
    {
        GetPlayfabFriends();
    }
    private void OnFriendAddedSuccess(AddFriendResult result)
    {
        GetPlayfabFriends();
    }

    private void OnFailure(PlayFabError error)
    {
        Debug.Log($"Playfab Friend Error occured: {error.GenerateErrorReport()}");
    }

    private void OnFriendsListSuccess(GetFriendsListResult result)
    {
        friends = result.Friends;
        OnFriendListUpdated?.Invoke(result.Friends);
        Debug.Log($"Friends List Success, {result.Friends.Count} Friends found.");
        
    }

    private void GetPlayfabFriends()
    {
        var request = new GetFriendsListRequest { XboxToken = null };
        PlayFabClientAPI.GetFriendsList(request, OnFriendsListSuccess, OnFailure);
    }
}
