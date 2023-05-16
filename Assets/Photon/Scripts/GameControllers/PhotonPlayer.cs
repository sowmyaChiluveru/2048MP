using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System;

public class PhotonPlayer : MonoBehaviour
{
    public static PhotonPlayer photonInfo;   
    public PhotonView PV;
   
    public void OnEnable()
    {
        if (photonInfo == null)
            photonInfo = this;
        else if(photonInfo!=this)
        {
            Destroy(photonInfo.gameObject);
            photonInfo = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {      
      
        PV = GetComponent<PhotonView>();
        if(PV.IsMine)
        {
           
           // PV.RPC("RPC_GenerateDeck", RpcTarget.AllBuffered, data);
        }
       
    }
   
   
    [PunRPC]
    public void RPC_PlayCards(List<string> cards)
    {

      
    }
}
