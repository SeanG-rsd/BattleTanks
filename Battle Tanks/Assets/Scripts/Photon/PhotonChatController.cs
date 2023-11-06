using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using Photon.Chat.Demo;
using ExitGames.Client.Photon;
using System;

public class PhotonChatController : MonoBehaviour, IChatClientListener
{
    [SerializeField] private string nickName;
    private ChatClient chatClient;

    public static Action<string, string> OnRoomInvite = delegate { };
    public static Action<ChatClient> OnChatConnected = delegate { };
    public static Action<PhotonStatus> OnStatusUpdated = delegate { };
    public static Action<string> OnFriendRequest = delegate { };
    public static Action<string> OnFriendRequestAccepted = delegate { };

    private static string friendRequest = "FRIENDINVITE!~0()";
    private static string friendAccept = "FRIENDACCEPT!~0()";

    // Start is called before the first frame update
    private void Awake()
    {
        chatClient = new ChatClient(this);
        nickName = PlayerPrefs.GetString("USERNAME");
        ConnectToPhotonChat();

        
        UIFriend.OnInviteFriend += HandleFriendInvite;
    }

    private void OnDestroy()
    {
        UIFriend.OnInviteFriend -= HandleFriendInvite;
    }
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        chatClient.Service();
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
    }

    public void HandleFriendInvite(string recipient)
    {
        chatClient.SendPrivateMessage(recipient, PhotonNetwork.CurrentRoom.Name);
        Debug.Log(recipient);
    }

    public void SendDirectMessage(string recipient, string message)
    {
        chatClient.SendPrivateMessage(recipient, message);
    }

    private void ConnectToPhotonChat()
    {
        Debug.Log("Connecting to Photon Chat");
        chatClient.AuthValues = new Photon.Chat.AuthenticationValues(nickName);
        Debug.Log(PlayerPrefs.GetString("USERNAME"));
        ChatAppSettings chatSettings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
        chatClient.ConnectUsingSettings(chatSettings);
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        
    }

    public void OnDisconnected()
    {
        Debug.Log("You have disconnected to the Photon Chat");
        chatClient.SetOnlineStatus(ChatUserStatus.Offline);
    }

    public void OnConnected()
    {
        Debug.Log("You have connected to the Photon Chat");
        OnChatConnected?.Invoke(chatClient);
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
    }

    public void OnChatStateChange(ChatState state)
    {
        
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        if (!string.IsNullOrEmpty(message.ToString()))
        {
            if (!message.Equals(friendAccept) && !message.Equals(friendRequest))
            {
                Debug.Log("recieved message");
                // Channel Name format [Sender : Recipient]
                string[] splitNames = channelName.Split(':');
                string senderName = splitNames[0];
                //Debug.Log(splitNames[0]);
                if (!sender.Equals(senderName, StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log($"{sender}: {message}");
                    OnRoomInvite?.Invoke(sender, message.ToString());
                }
            }
            else if (message.Equals(friendRequest))
            {
                Debug.Log("recieved friend invite");
                string[] splitNames = channelName.Split(':');
                string senderName = splitNames[0];

                if (!sender.Equals (senderName, StringComparison.OrdinalIgnoreCase))
                {
                    OnFriendRequest?.Invoke(sender);
                }
            }
            else if (message.Equals(friendAccept))
            {
                Debug.Log("recieve friend accept");
                string[] splitNames = channelName.Split(':');
                string senderName = splitNames[0];

                if (!sender.Equals(senderName, StringComparison.OrdinalIgnoreCase))
                {
                    OnFriendRequestAccepted?.Invoke(sender);
                }
            }
        }

        Debug.LogError($"on private message from: {sender}");
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        
    }

    public void OnUnsubscribed(string[] channels)
    {
        
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log($"Photon Chat OnStatusUpdate: {user} changed to {status}: {message}");
        PhotonStatus newStatus = new PhotonStatus(user, status, (string)message);
        Debug.Log($"Status Update for {user} and its not {status}.");
        Debug.Log(ChatUserStatus.Online);
        OnStatusUpdated?.Invoke(newStatus);
    }

    public void OnUserSubscribed(string channel, string user)
    {
       
    }

    public void OnUserUnsubscribed(string channel, string user)
    {

    }
}
