using TMPro;
using Photon.Realtime;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class UIPlayerSelection : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text userNameText;
    [SerializeField] private Player owner;
    

    public Player Owner
    {
        get { return owner; }
        private set { owner = value; }
    }

    public void Initialize(Player player)
    {
        Owner = player;
        SetupPlayerSelection();

    }

    private void SetupPlayerSelection()
    {
        userNameText.SetText(owner.NickName);
    }
}
