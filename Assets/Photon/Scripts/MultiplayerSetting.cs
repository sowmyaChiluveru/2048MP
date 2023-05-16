using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerSetting : MonoBehaviour
{
    public static MultiplayerSetting multiSetting;
  public  bool delay;
   public int maxPlayers;
   public  int menuScene;
   public int multiPlayerScene;

    void Awake()
    {
        if (multiSetting == null)
            multiSetting = this;
        else if(multiSetting != this)
        {
            Destroy(multiSetting.gameObject);
            multiSetting = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
    
}
