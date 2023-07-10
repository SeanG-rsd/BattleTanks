using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDisplayFriends : MonoBehaviour
{
    [SerializeField] private Transform friendContainer;
    [SerializeField] private UIFriend uiFriendPrefab;
    private void Awake()
    {
        PhotonChatFriendController.OnDisplayFriends += HandleDisplayFriends;
    }

    private void OnDestroy()
    {
        PhotonChatFriendController.OnDisplayFriends -= HandleDisplayFriends;
    }

    private void HandleDisplayFriends(List<string> friends)
    {
        //Debug.LogWarning(friends.Count);
        foreach(Transform child in friendContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (string friend in friends)
        {
            UIFriend uiFriend = Instantiate(uiFriendPrefab, friendContainer);
            uiFriend.Initialize(friend);
        }
    }
}
