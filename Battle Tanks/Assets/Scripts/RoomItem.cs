using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    public TMP_Text roomName;
    CreateAndJoinRooms manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<CreateAndJoinRooms>();
    }

    public void SetRoomName(string _roomName)
    {
        roomName.text = _roomName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickItem()
    {
        manager.JoinRoom(roomName.text);
    }
}
