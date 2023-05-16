using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour
{

    public Text roomName;
    public Text roomsize;


    public  void SetText(string roomNameText,int size)
    {
        roomName.text = roomNameText;
        roomsize.text = size.ToString();
    }

   public void JoinRoomOnClick()
    {
        PhotonNetwork.JoinRoom(roomName.text);
        Debug.Log("the room name" + roomName.text);
    }
}
