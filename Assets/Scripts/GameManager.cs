using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject gamePanel1;
    public GamePanel gamePanelScript1;
    public GameObject gamePanel2;
    public GamePanel gamePanelScript2;
    public TileStyleHolder tileStyleHolder;
    public Transform blockerTileObjToTakeFrom;  
    public Transform blockerTilePanel;
    public List<Tile> blockersList = new List<Tile>();
    public GameObject winPanel;
    public Text winLoseText;


    void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this as GameManager;
        }
        //if (Instance==null)
        //{
        //    Instance = this;
        //}
        //else if (Instance != this)
        //{
        //    Destroy(this.gameObject);
        //    Instance = this;
        //}

    }

    public void OnPlayAgainButtonAction()
    {
        Destroy(PhotonRoom.room.gameObject);
        StartCoroutine(OnDisconnected());
    }

    IEnumerator OnDisconnected()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.AutomaticallySyncScene = false;
        Debug.Log("The value made fale");
        PhotonNetwork.LeaveLobby();
        if (PhotonNetwork.InRoom)
            yield return null;
        //yield return new WaitForSeconds(0.5f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(MultiplayerSetting.multiSetting.menuScene);
    }

    public void EndOfGame()
    {
        string winner = "";
        int score = -1;

        List<Player> plys = PhotonNetwork.PlayerList.ToList();
        List<Player> p = plys.OrderByDescending(x => x.GetScore()).ToList();
        winner = p[0].NickName;
        score = p[0].GetScore();
        Debug.Log("the score of player ::" + score);
        StartCoroutine(EndOfGame(winner, score));
    }

    private IEnumerator EndOfGame(string winner, int score)
    {
        // float timer = 5.0f;
        winPanel.SetActive(true);
        winLoseText.text = string.Format("Player {0} won with {1} points.", winner, score);       
        yield return null;
    }

}
