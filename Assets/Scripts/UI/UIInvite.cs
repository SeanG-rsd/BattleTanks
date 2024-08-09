using System;
using TMPro;
using UnityEngine;

public class UIInvite : MonoBehaviour
{
    [SerializeField] private string friendName;
    [SerializeField] private string roomName;
    [SerializeField] private TMP_Text friendNameText;

    public static Action<UIInvite> OnInviteAccept = delegate { };
    public static Action<string> OnRoomInviteAccept = delegate { };
    public static Action<UIInvite> OnInviteDecline = delegate { };
    public void Initialize(string friendName, string roomName)
    {
        this.friendName = friendName;
        this.roomName = roomName;

        friendNameText.text = friendName;
    }

    public void AcceptInvite()
    {
        OnInviteAccept?.Invoke(this);
        OnRoomInviteAccept?.Invoke(roomName);
    }

    public void DeclineInvite()
    {
        OnInviteDecline?.Invoke(this);
    }
}
