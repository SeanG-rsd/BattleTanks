using UnityEngine;
using System;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;
using Photon.Chat;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class UIFriend : MonoBehaviour
{

    [SerializeField] private TMP_Text friendNameText;
    [SerializeField] private string friendName;
    [SerializeField] private bool isOnline;
    [SerializeField] private Image onlineImage;
    [SerializeField] private GameObject inviteButton;
    [SerializeField] private Color onlineColor;
    [SerializeField] private Color offlineColor;

    [SerializeField] private Sprite inviteImage;
    [SerializeField] private Sprite regularImage;


    public static Action<string> OnRemoveFriend = delegate { };
    public static Action<string> OnInviteFriend = delegate { };
    public static Action<string> OnGetCurrentStatus = delegate { };
    public static Action OnGetRoomStatus = delegate { };


    private void Awake()
    {
        PhotonChatController.OnStatusUpdated += HandleStatusUpdated;
        PhotonChatFriendController.OnStatusUpdated += HandleStatusUpdated;
        PhotonRoomController.OnRoomStatusChange += HandleInRoom;
    }

    private void OnDestroy()
    {
        PhotonChatController.OnStatusUpdated -= HandleStatusUpdated;
        PhotonChatFriendController.OnStatusUpdated -= HandleStatusUpdated;
        PhotonRoomController.OnRoomStatusChange += HandleInRoom;
    }

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(friendName)) return;
        OnGetCurrentStatus?.Invoke(friendName);
        OnGetRoomStatus?.Invoke();
    }

    public void Initialize(FriendInfo friend)
    {
        SetupUI();
    }

    public void Initialize(string friendName)
    {
        this.friendName = friendName;
        SetupUI();
        OnGetCurrentStatus?.Invoke(friendName);
        OnGetRoomStatus?.Invoke();
    }

    private void HandleStatusUpdated(PhotonStatus status)
    {
        if (string.Compare(friendName, status.PlayerName) == 0)
        {
            //Debug.Log(status.Status);
            SetStatus(status.Status);
        }
    }

    private void HandleInRoom(bool inRoom)
    {
        //Debug.Log("alkdjf");
        inviteButton.SetActive(inRoom && isOnline);
        gameObject.GetComponent<Image>().sprite = (inRoom && isOnline) ? inviteImage : regularImage;
        
    }

    private void SetupUI()
    {
        friendNameText.SetText(friendName);
        inviteButton.SetActive(false);
        gameObject.GetComponent<Image>().sprite = regularImage;
    }

    private void SetStatus(int status)
    {
       // Debug.Log("Set Status");
        if (status ==ChatUserStatus.Online)
        {
            gameObject.GetComponent<Image>().sprite = inviteImage;
            inviteButton.SetActive(true);
            isOnline = true;
            OnGetRoomStatus?.Invoke();
        }
        else
        {
            isOnline = false;
            inviteButton.SetActive(false);
            gameObject.GetComponent<Image>().sprite = regularImage;
        }
    }
       

    public void RemoveFriend()
    {
        OnRemoveFriend?.Invoke(friendName);
    }

    public void InviteFriend()
    {
        //Debug.Log($"Clicked to invite friend {friendName}");
        OnInviteFriend?.Invoke(friendName);
    }
}
