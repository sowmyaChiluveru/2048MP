
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using Hashtable = ExitGames.Client.Photon.Hashtable;

using System.IO;

using UnityEngine.UI;

public class PhotonRoom : MonoBehaviourPunCallbacks,IInRoomCallbacks
{
    #region CUSTOMMATCHING
   public GameObject lobbyGO;
    public  GameObject roomGo;
    public Transform playersPanel;
    public GameObject playerListingPrefab;
    public GameObject startButton;
    #endregion

    //Room info
    public static PhotonRoom room;
    PhotonView pV;
    public int currentScene;
    public int multiPlayScene;

    //Player info
    public Player[] photonPlayers;
    public int playersInRoom;
    public int myNumberInRoom;
  public   int playerInGame;
    public GameObject localPlayer;

    //Delayed info
    public bool readyToCount;
    public bool readyToStart;
    public float startingTime;
    public float lessThanMaxPlayers;
    public float atMaxPlayers;
    public float timeToStart;
   

    bool isGameLoaded = false;


    //data passing
  
    string cardDataAsString;
    void Awake()
    {
        if (room == null)
            room = this;
        else if(room!=this)
        {
            Destroy(room.gameObject);
            room = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }



    // Start is called before the first frame update
    void Start()
    {
        pV = GetComponent<PhotonView>();
        readyToCount = false;
        readyToStart = false;
        lessThanMaxPlayers = startingTime;
        atMaxPlayers = 6;
        timeToStart = startingTime;
    }
    void Update()
    {
        //if(MultiplayerSetting.multiSetting.delay)
        //{
        //    if (playersInRoom == 1)
        //    {
        //        reStartTimer();
        //    }
        //    if (!isGameLoaded)
        //    { 
        //        if(readyToStart)
        //        {
        //            atMaxPlayers -= Time.deltaTime;
        //            lessThanMaxPlayers = atMaxPlayers;
        //            timeToStart = atMaxPlayers;
                   
        //        }
        //        else if(readyToCount)
        //        {
        //            lessThanMaxPlayers -= Time.deltaTime;
        //            timeToStart = lessThanMaxPlayers;
        //        }
        //        //Debug.LogFormat("ready to strart {0}, ready to count{1}, timetostart {2}", readyToStart,readyToCount, timeToStart);
        //        if (timeToStart<=0)
        //        {
        //            StartGame();
        //        }
        //    }
        //}
    }
    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {               
        currentScene = scene.buildIndex;
        if(currentScene==MultiplayerSetting.multiSetting.multiPlayerScene)
        {
            isGameLoaded = true;
            if(MultiplayerSetting.multiSetting.delay)
            {
                pV.RPC("RPC_SendData", RpcTarget.MasterClient);
            }
            else
            {
                RPC_CreateData();
            }
        }
    }

  
    [PunRPC]
    void RPC_SendData()
    {
        playerInGame++;       
        if (playerInGame==PhotonNetwork.PlayerList.Length)
        {
          
            //pV.RPC("RPC_CreateData", PhotonNetwork.PlayerList[1]);
            pV.RPC("RPC_CreateData", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void RPC_CreateData()
    {
        Debug.Log("instantiated::");
        Vector3 pos=Vector3.zero;//Solitaire.Instance.hud.playersHolder.transform.position;
        localPlayer= PhotonNetwork.Instantiate("GamePanel", pos, Quaternion.identity, 0) as GameObject;
       // PhotonView childPV = localPlayer.GetComponent<PhotonView>();
        //childPV.RPC("RPC_SetData", RpcTarget.AllBuffered,PhotonNetwork.NickName);
    }
   
    //for normal Functionality or else use custom matching which is below
    //public override void OnJoinedRoom()
    //{
    //    base.OnJoinedRoom();
    //    Debug.Log("Joined the room");
    //    photonPlayers = PhotonNetwork.PlayerList;
    //    playersInRoom = photonPlayers.Length;
    //    myNumberInRoom = playersInRoom;
    //    //PhotonNetwork.NickName = myNumberInRoom.ToString();
       
    //    if(MultiplayerSetting.multiSetting.delay)
    //    {
    //        if(playersInRoom>1)
    //        {
    //            readyToCount = true;
    //        }
    //        if(playersInRoom==MultiplayerSetting.multiSetting.maxPlayers)
    //        {
    //            readyToStart = true;
    //            if (!PhotonNetwork.IsMasterClient)
    //                return;
    //            PhotonNetwork.CurrentRoom.IsOpen = false;
    //        }
    //    }
    //    else
    //    {
    //        StartGame();
    //    }
        
    //}

  public void StartGame()
    {
        isGameLoaded = true;
        if (!PhotonNetwork.IsMasterClient)
            return;
        if(MultiplayerSetting.multiSetting.delay)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        PhotonNetwork.LoadLevel(MultiplayerSetting.multiSetting.multiPlayerScene);
    }

    void reStartTimer()
    {
        lessThanMaxPlayers = startingTime;
        atMaxPlayers = 6;
        timeToStart = startingTime;
        readyToCount = false;
        readyToStart = false;
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("player entered  the room");
        ClearRoomListing();
        ListRooms();

        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom++;
        if (MultiplayerSetting.multiSetting.delay)
        {
            if (playersInRoom > 1)
            {
                readyToCount = true;
            }
            if (playersInRoom == MultiplayerSetting.multiSetting.maxPlayers)
            {
                readyToStart = true;
                if (!PhotonNetwork.IsMasterClient)
                    return;
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
       
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        if (playersPanel != null)
        {
            ClearRoomListing();
            ListRooms();
        }
        playersInRoom--;
        Debug.Log("the player left the room :::" + otherPlayer.NickName);
    }
 

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("room joined failed" + message);
        base.OnJoinRoomFailed(returnCode, message);
    }
    #region CUSTOMMATCHING
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined the room"+PhotonNetwork.CurrentRoom.Name);

        lobbyGO.SetActive(false);
        roomGo.SetActive(true);
        if(PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
        ClearRoomListing();
        ListRooms();

        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        myNumberInRoom = playersInRoom;
        //PhotonNetwork.NickName = myNumberInRoom.ToString();

        if (MultiplayerSetting.multiSetting.delay)
        {
            if (playersInRoom > 1)
            {
                readyToCount = true;
            }
            if (playersInRoom == MultiplayerSetting.multiSetting.maxPlayers)
            {
                readyToStart = true;
                if (!PhotonNetwork.IsMasterClient)
                    return;
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
    }

    void ClearRoomListing()
    {
        
        for(int i=playersPanel.childCount-1;i>=0;i--)
        {
            Destroy(playersPanel.GetChild(i).gameObject);
        }
    }

    void ListRooms()
    {
        if (PhotonNetwork.InRoom)
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                GameObject playerObj = Instantiate(playerListingPrefab, playersPanel);
                Text playerText = playerObj.transform.GetChild(0).GetComponent<Text>();
                playerText.text = p.NickName;
            }
        }
    }
    #endregion


}
