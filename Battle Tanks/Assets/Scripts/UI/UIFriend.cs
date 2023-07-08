using UnityEngine;
using System;
using TMPro;
using Photon.Realtime;

public class UIFriend : MonoBehaviour
{

    [SerializeField] private TMP_Text friendNameText;
    [SerializeField] private FriendInfo friend;

    public static Action<string> OnRemoveFriend = delegate { };
    public static Action<string> OnInviteFriend = delegate { };

    public void Initialize(FriendInfo friend)
    {
        this.friend = friend;
        friendNameText.SetText(this.friend.UserId);
    }

    public void RemoveFriend()
    {
        OnRemoveFriend?.Invoke(friend.UserId);
    }

    public void InviteFriend()
    {
        Debug.Log($"Clicked to invite friend {friend.UserId}");
        OnInviteFriend?.Invoke(friend.UserId);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
