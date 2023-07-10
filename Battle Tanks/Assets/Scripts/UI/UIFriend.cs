using UnityEngine;
using System;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;
using Photon.Chat;

public class UIFriend : MonoBehaviour
{

    [SerializeField] private TMP_Text friendNameText;
    [SerializeField] private string friendName;
    [SerializeField] private bool isOnline;
    [SerializeField] private Image onlineImage;
    [SerializeField] private GameObject inviteButton;
    [SerializeField] private Color onlineColor;
    [SerializeField] private Color offlineColor;


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
            SetStatus(status.Status);
        }
    }

    private void HandleInRoom(bool inRoom)
    {
        inviteButton.SetActive(inRoom && isOnline);
    }

    private void SetupUI()
    {
        friendNameText.SetText(friendName);
        inviteButton.SetActive(false);
    }

    private void SetStatus(int status)
    {
        if (status ==ChatUserStatus.Online)
        {
            onlineImage.color = onlineColor;
            isOnline = true;
            OnGetRoomStatus?.Invoke();
        }
        else
        {
            onlineImage.color = offlineColor;
            isOnline = false;
            inviteButton.SetActive(false);
        }
    }
       

    public void RemoveFriend()
    {
        OnRemoveFriend?.Invoke(friendName);
    }

    public void InviteFriend()
    {
        Debug.Log($"Clicked to invite friend {friendName}");
        OnInviteFriend?.Invoke(friendName);
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
