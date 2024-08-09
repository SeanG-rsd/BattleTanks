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

    public string[] avatars;

    private const string playAv = "playerAvatar";

    private void Awake()
    {
        TankCarosel.OnTankSelected += HandleTankSelected;
    }

    private void OnDestroy()
    {
        TankCarosel.OnTankSelected -= HandleTankSelected;
    }
    public void Initialize(Player player)
    {
        userNameText.text = PhotonNetwork.LocalPlayer.NickName;
        SetupPlayerSelection(0);
    }

    private void SetupPlayerSelection(int tank)
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { playAv, tank } });
    }

    private void HandleTankSelected(string tank)
    {
        if (tank == "Light Tank")
        {
            SetupPlayerSelection(1);
        }
        else if (tank == "Heavy Tank")
        {
            SetupPlayerSelection(0);
        }
        else if (tank == "Normal Tank")
        {
            SetupPlayerSelection(2);
        }
    }
}
