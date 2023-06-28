using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class PlayerItem : MonoBehaviourPunCallbacks
{
    public TMP_Text playerName;

    public Image teamBackgroundImage;
    public Color[] teamColors;
    public Image backgroundImage;
    public Color highlightColor;
    public GameObject leftAvatarArrowButton;
    public GameObject rightAvatarArrowButton;

    public GameObject leftTeamArrowButton;
    public GameObject rightTeamArrowButton;

    ExitGames.Client.Photon.Hashtable playerPropeties = new ExitGames.Client.Photon.Hashtable();
    public TMP_Text playerAvatar;
    public string[] avatars;

    public TMP_Text playerTeam;
    public string[] teamNames;

    Player player;
    CreateAndJoinRooms manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<CreateAndJoinRooms>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlayInfo(Player _player)
    {
        playerName.text = _player.NickName;
        player = _player;
        Debug.Log("new player");
        UpdatePlayerItem(player);
    }

    public void ApplyLocalChanges()
    {
        backgroundImage.color = highlightColor;
        leftAvatarArrowButton.SetActive(true);
        rightAvatarArrowButton.SetActive(true);
        leftTeamArrowButton.SetActive(true);
        rightTeamArrowButton.SetActive(true);
    }

    public void OnClickLeftArrow()
    {
        if ((int)playerPropeties["playerAvatar"] == 0)
        {
            playerPropeties["playerAvatar"] = avatars.Length - 1;
        }
        else
        {
            playerPropeties["playerAvatar"] = (int)playerPropeties["playerAvatar"] - 1;
        }
        PhotonNetwork.SetPlayerCustomProperties(playerPropeties);
    }

    public void OnClickRightArrow()
    {
        if ((int)playerPropeties["playerAvatar"] == avatars.Length - 1)
        {
            playerPropeties["playerAvatar"] = 0;
        }
        else
        {
            playerPropeties["playerAvatar"] = (int)playerPropeties["playerAvatar"] + 1;
        }
        PhotonNetwork.SetPlayerCustomProperties(playerPropeties);
    }

    public void OnClickLeftTeamArrow()
    {
        if ((int)playerPropeties["playerTeam"] == 0)
        {
            playerPropeties["playerTeam"] = teamNames.Length - 1;
        }
        else
        {
            playerPropeties["playerTeam"] = (int)playerPropeties["playerTeam"] - 1;
        }
        PhotonNetwork.SetPlayerCustomProperties(playerPropeties);
    }

    public void OnClickRightTeamArrow()
    {
        if ((int)playerPropeties["playerTeam"] == teamNames.Length - 1)
        {
            playerPropeties["playerTeam"] = 0;
        }
        else
        {
            playerPropeties["playerTeam"] = (int)playerPropeties["playerTeam"] + 1;
        }
        PhotonNetwork.SetPlayerCustomProperties(playerPropeties);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (player == targetPlayer)
        {
            UpdatePlayerItem(targetPlayer);
        }

        if (targetPlayer.CustomProperties.ContainsKey("choseGame"))
        {
            manager.choseGame = true;
        }
    }

    void UpdatePlayerItem(Player player)
    {
        if (player.CustomProperties.ContainsKey("playerAvatar"))
        {
            playerAvatar.text = avatars[(int)player.CustomProperties["playerAvatar"]];
            playerPropeties["playerAvatar"] = (int)player.CustomProperties["playerAvatar"];
        }
        else
        {
            playerPropeties["playerAvatar"] = 0;
        }

        if (player.CustomProperties.ContainsKey("playerTeam"))
        {
            playerTeam.text = teamNames[(int)player.CustomProperties["playerTeam"]];
            playerPropeties["playerTeam"] = (int)player.CustomProperties["playerTeam"];
            teamBackgroundImage.color = teamColors[(int)player.CustomProperties["playerTeam"]];
        }
        else
        {
            playerPropeties["playerTeam"] = 0;
        }

       
    }
}
