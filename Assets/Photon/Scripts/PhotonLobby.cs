using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonLobby : MonoBehaviourPunCallbacks,ILobbyCallbacks
{
    #region CUSTOMMATCHING
    public GameObject roomListingPrefab;
    public Transform roomsPanel;
    public List<RoomInfo> roomListing;
    #endregion

    public static PhotonLobby lobby;

    public GameObject connectButton;
    public GameObject cancelButton;
    void Awake()
    {
        lobby = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        //connects to photon master,and it gives feedback regarding the connections i.e.,
        //some callbacks are used to take the feedbacks.

        PhotonNetwork.ConnectUsingSettings();
        roomListing = new List<RoomInfo>();
    }

    public override void OnConnectedToMaster()
    {      
        //base.OnConnectedToMaster();
        Debug.Log("the player connected to master photon server");
        PhotonNetwork.AutomaticallySyncScene = true;
        connectButton.SetActive(true);
    }

    public  void OnConnectButtonClicked()
    {
        connectButton.SetActive(false);
        cancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
    }

   
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("on joinRandom room failed");
        //base.OnJoinRandomFailed(returnCode, message);
        CreateRoom();
    }

    void CreateRoom()
    {
        int range = Random.Range(0, 10000);
        RoomOptions roomOpt = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers =(byte)MultiplayerSetting.multiSetting.maxPlayers ,CleanupCacheOnLeave=false,PublishUserId=true};
        PhotonNetwork.CreateRoom("Room" + range, roomOpt);
    }

    
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("on create room failed");
        CreateRoom();
    }

    #region #region CUSTOMMATCHING
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
       // RemoveRoomsFromListing();

        int temp = -1;
    

        foreach(RoomInfo room in roomList)
        {
            if(roomListing!=null)
            {
                //temp = roomListing.FindIndex(x =>x.Name==room.Name);
                temp = roomListing.FindIndex(ByName(room.Name));
            }
            else
            {
                temp = -1;
            }if(temp!=-1)
            {
                roomListing.RemoveAt(temp);
                Destroy(roomsPanel.GetChild(temp).gameObject);
            }
            else
            {
                roomListing.Add(room);
                ListRoom(room);
            }
           
        }
    }

    System.Predicate<RoomInfo> ByName(string name)
    {
        return delegate (RoomInfo _room)
        {
           return _room.Name == name;
        };
    }
    void RemoveRoomsFromListing()
    {
        for (int i = roomsPanel.childCount - 1; i >= 0; i--)
        {
            Destroy(roomsPanel.GetChild(i).gameObject);
        }

        //while(roomsPanel.childCount>0)
        //{
        //    Destroy(roomsPanel.GetChild(0).gameObject);
        //}
    }
    void ListRoom(RoomInfo _room)
    {
        if (_room.IsOpen && _room.IsVisible)
        {
            GameObject roomObj = Instantiate(roomListingPrefab, roomsPanel);
            RoomButton roomBtn = roomObj.GetComponent<RoomButton>();
           // roomBtn.roomNameText = _room.Name;
            roomBtn.SetText(_room.Name,(int)_room.MaxPlayers);
        }

    }

   public  void CreateCustomMatchingRoom()
    {
        int range = Random.Range(1000000, 9999999);
        RoomOptions roomOpt = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)4 };
        PhotonNetwork.CreateRoom(range.ToString(), roomOpt);
    }

    public void JoinLobbyOnClick()
    {
        if(!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    #endregion


    public void CancelButtonAction()
    {
        connectButton.SetActive(true);
        cancelButton.SetActive(false);
        PhotonNetwork.LeaveRoom();
    }





}
