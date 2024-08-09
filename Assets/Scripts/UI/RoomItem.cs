using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    public TMP_Text roomName;
    PhotonRoomController manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<PhotonRoomController>();
    }

    public void SetRoomName(string _roomName)
    {
        roomName.text = _roomName;
    }

    public void OnClickItem()
    {
        manager.JoinPhotonRoom(roomName.text);
    }
}
