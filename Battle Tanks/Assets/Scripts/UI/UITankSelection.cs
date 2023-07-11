using TMPro;
using Photon.Realtime;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class UITankSelection : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text userNameText;
    [SerializeField] private Player owner;

    Hashtable playerPropeties = new Hashtable();
    [SerializeField] TMP_Text playerAvatar;
    public string[] avatars;

    TeamInfo teamInfo;

    private const string playAv = "playerAvatar";


    public Player Owner
    {
        get { return owner; }
        private set { owner = value; }
    }

    public void Initialize(Player player)
    {
        playerAvatar = GameObject.Find("UITankSelect").transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        teamInfo = GetComponent<TeamInfo>();
        Owner = player;
        playerPropeties = PhotonNetwork.LocalPlayer.CustomProperties;
        
        SetupPlayerSelection();

    }

    private void SetupPlayerSelection()
    {
        userNameText.SetText(owner.NickName);
        playerPropeties[playAv] = 0;
        playerAvatar.SetText(avatars[(int)playerPropeties[playAv]]);
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerPropeties);
    }

    public void OnClickLeftArrow()
    {
        if (playerPropeties.ContainsKey(playAv))
        {
            if ((int)playerPropeties[playAv] == 0)
            {
                playerPropeties[playAv] = avatars.Length - 1;

            }
            else
            {
                playerPropeties[playAv] = (int)playerPropeties[playAv] - 1;

            }
        }
        else
        {
            playerPropeties[playAv] = 0;
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerPropeties);
        playerAvatar.SetText(avatars[(int)playerPropeties[playAv]]);
    }

    public void OnClickRightArrow()
    {
        if (playerPropeties.ContainsKey(playAv))
        {
            if ((int)playerPropeties[playAv] == avatars.Length - 1)
            {
                playerPropeties[playAv] = 0;
            }
            else
            {
                playerPropeties[playAv] = (int)playerPropeties[playAv] + 1;
            }
        }
        else
        {
            playerPropeties[playAv] = 0;
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerPropeties);
        playerAvatar.SetText(avatars[(int)playerPropeties[playAv]]);
    }
}
