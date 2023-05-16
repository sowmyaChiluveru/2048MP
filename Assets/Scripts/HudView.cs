using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class HudView : MonoBehaviourPunCallbacks
{
    public static HudView Instance;  
    public int score;
    public Text scoreText;
 
    public const string PlayerScoreProp = "score";
    void Awake()
    {       
        Instance = this;
    }

    //void Update()
    //{
    //    if (!timeStarted)
    //        return;
    //    if (timer > 0)
    //    {
    //        float minutes = Mathf.Floor(timer / 60);
    //        float seconds = timer % 60;
    //        if (minutes < 1)
    //            timeText.color = Color.red;

    //        timeText.text = minutes + ":" + Mathf.RoundToInt(seconds);
    //        timer -= Time.deltaTime;
    //    }
    //    else
    //    {          
    //        timeStarted = false;
    //    }
    //}

 
    public void UpdateScore(int _score,PhotonView _pv)
    {      
       // PhotonView pView = GameManager.Instance.gamePanelScript1.GetComponent<PhotonView>();
        _pv.Owner.AddScore(_score);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {        
        if (targetPlayer.IsLocal)
        {
           scoreText.text = targetPlayer.GetScore().ToString();          
        }
        //List<PhotonView> childs = playersHolder.GetComponentsInChildren<PhotonView>().ToList();
        //if(childs!=null)
        //{
        //    PhotonView pV = childs.Find(x => x.Owner.NickName == targetPlayer.NickName);
        //    if(pV)
        //    {
        //      //  pV.gameObject.GetComponent<PlayerDetails>().scoreText.text = targetPlayer.GetScore().ToString();
        //    }
        //}
      
    }
}

public static class ScoreData
{
    public static void SetScore(this Player player, int newScore)
    {
        Hashtable score = new Hashtable();  // using PUN's implementation of Hashtable
        score[HudView.PlayerScoreProp] = newScore;

        player.SetCustomProperties(score);  // this locally sets the score and will sync it in-game asap.
    }

    public static void AddScore(this Player player, int scoreToAddToCurrent)
    {
        int current = player.GetScore();
        current = current + scoreToAddToCurrent;

        Hashtable score = new Hashtable();  // using PUN's implementation of Hashtable
        score[HudView.PlayerScoreProp] = current;
        Debug.Log("the score of plyer in addscore::" + current);
        player.SetCustomProperties(score);  // this locally sets the score and will sync it in-game asap.
    }

    public static int GetScore(this Player player)
    {
        object score;
        if (player.CustomProperties.TryGetValue(HudView.PlayerScoreProp, out score))
        {
            Debug.Log("the score of plyer in GetScore::" + (int)score);
            return (int)score;
        }
        return 0;
    }
}