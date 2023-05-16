using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using DG.Tweening;
using Hashtable = ExitGames.Client.Photon.Hashtable;



//[SerializeField]
public class TileData
{
    public int id;
    public MoveDirection movDirection;
    public string tileDt;
    public int valueNumber;
    public bool generateBlock;
    
    public TileData()
    {
    }

    public TileData(int _id,MoveDirection md,string tDt,int valueno,bool _genBlock=false)
    {
        id = _id;
        movDirection = md;
        tileDt = tDt;
        valueNumber = valueno;
        generateBlock = _genBlock;
       
    }
}
public class GamePanel : MonoBehaviour, IPunObservable
{

    public PhotonView pV;

    public Tile[,] allTiles = new Tile[4, 4];
    public List<Tile> emptyTiles = new List<Tile>();
    List<Tile[]> columns = new List<Tile[]>();
    List<Tile[]> rows = new List<Tile[]>();
    string data; //sending the index and number for generating num
    MoveDirection mvDr = MoveDirection.None;
    public Queue<TileData> dataQueue = new Queue<TileData>();
    public Queue<TileData> sendingDataQueue = new Queue<TileData>();
    int valueSent = 0; //for every move incrementing 
    bool isTimerRunning = false;
    bool isFirstIndex = false;
    void Awake()
    {
        Debug.Log("the onenable has been called");
        if (pV.IsMine)//&&pV.Owner.IsLocal
        {
            this.transform.SetParent(GameManager.Instance.gamePanel1.transform, false);
            GameManager.Instance.gamePanelScript1 = this;
        }
        else
        {
            this.transform.SetParent(GameManager.Instance.gamePanel2.transform, false);
            GameManager.Instance.gamePanelScript2 = this;
        }
    }

    public  void OnEnable()
    {      
        TimeCounter.onCountDownTimerExpire += OnTimerCompleteAction;
    }
    public void OnDisable()
    {
        TimeCounter.onCountDownTimerExpire -= OnTimerCompleteAction;
    }
    // Start is called before the first frame update
    void Start()
    {
        List<Tile> tiles = this.GetComponentsInChildren<Tile>().ToList();

        foreach (Tile t in tiles)
        {
            t.Number = 0;
            allTiles[t.Row, t.Col] = t;
            emptyTiles.Add(t);
        }
        columns.Add(new Tile[] { allTiles[0, 0], allTiles[1, 0], allTiles[2, 0], allTiles[3, 0] });
        columns.Add(new Tile[] { allTiles[0, 1], allTiles[1, 1], allTiles[2, 1], allTiles[3, 1] });
        columns.Add(new Tile[] { allTiles[0, 2], allTiles[1, 2], allTiles[2, 2], allTiles[3, 2] });
        columns.Add(new Tile[] { allTiles[0, 3], allTiles[1, 3], allTiles[2, 3], allTiles[3, 3] });

        rows.Add(new Tile[] { allTiles[0, 0], allTiles[0, 1], allTiles[0, 2], allTiles[0, 3] });
        rows.Add(new Tile[] { allTiles[1, 0], allTiles[1, 1], allTiles[1, 2], allTiles[1, 3] });
        rows.Add(new Tile[] { allTiles[2, 0], allTiles[2, 1], allTiles[2, 2], allTiles[2, 3] });
        rows.Add(new Tile[] { allTiles[3, 0], allTiles[3, 1], allTiles[3, 2], allTiles[3, 3] });

        StartCoroutine(GenerateNum());
    }
    void Update()
    {
        if (dataQueue.Count > 0 && !moveMade)
        {
            TileData tile = dataQueue.Peek();
            if (tile != null)
            {
                if (tile.generateBlock)
                {
                    Debug.Log("the value s truee:::"+ tile.tileDt);
                    GenerateAndRemoveBlockTileToSetTimer(tile.tileDt,tile.id);
                }
                else
                {
                    Move(tile.movDirection, tile.tileDt, tile.valueNumber);
                }
                dataQueue.Dequeue();
            }
        }
    }
    IEnumerator GenerateNum()
    {
        yield return new WaitForSeconds(1f);
        if (pV.IsMine)
        {
            data = string.Empty;
            Generate();
            Generate();
            pV.RPC("_RPCSetData", RpcTarget.AllBuffered, data);
        }
    }

    [PunRPC]
    void _RPCSetData(string _data)
    {

        if (!pV.IsMine)
        {
            Debug.Log("the count:::" + GameManager.Instance.gamePanelScript2.emptyTiles.Count + ":::" + GameManager.Instance.gamePanelScript1.emptyTiles.Count);
            string[] splitdata = _data.Split('|');
            Debug.Log("the data:::" + int.Parse(splitdata[0]) + "::" + int.Parse(splitdata[1]) + "::" + int.Parse(splitdata[2]) + "::" + int.Parse(splitdata[3]));
            int index1 = int.Parse(splitdata[0]);
            int num1 = int.Parse(splitdata[1]);
            int index2 = int.Parse(splitdata[2]);
            int num2 = int.Parse(splitdata[3]);


            List<Tile> _empty = pV.gameObject.GetComponent<GamePanel>().emptyTiles;
            Debug.Log("the index1:::" + index1 + ":::" + index2 + ":::" + _empty.Count);
            _empty[index1].Number = num1;
            _empty.RemoveAt(index1);
            _empty[index2].Number = num2;
            _empty.RemoveAt(index2);
        }
    }
    public void Generate()
    {
        if (emptyTiles.Count > 0)
        {
            int index = Random.Range(0, emptyTiles.Count - 1);
            int random = Random.Range(0, 9);
            emptyTiles[index].Number = random > 3 ? 2 : 4;
            if (string.IsNullOrEmpty(data))
                data += index.ToString() + "|";
            else
                data += "|" + index.ToString() + "|";
            data += emptyTiles[index].Number.ToString();

            emptyTiles.RemoveAt(index);
        }
    }

    void ResetMergerFlags()
    {
        if (allTiles.Length > 0)
        {
            foreach (Tile t in allTiles)
            {
                // Debug.Log("the count::" + t);
                t.mergerThisTile = false;
            }
        }
    }
    void UpdateEmptyTiles()
    {
        emptyTiles.Clear();
        foreach (Tile t in allTiles)
        {
            if (t.Number == 0)
            {
                emptyTiles.Add(t);
            }
        }
    }

    bool MakeOneMoveDownIndex(Tile[] LinOfTiles)
    {
        // Debug.Log("the linoftiles:::MakeOneMoveDownIndex::;" + LinOfTiles.Length);
        for (int i = 0; i < LinOfTiles.Length - 1; i++)
        {
            if (LinOfTiles[i].Number == 0 && LinOfTiles[i + 1].Number != 0)
            {
                LinOfTiles[i].isOppBlock = LinOfTiles[i + 1].isOppBlock;
                LinOfTiles[i].Number = LinOfTiles[i + 1].Number;                
                LinOfTiles[i + 1].Number = 0;
                return true;
            }
            bool canMerge = ((LinOfTiles[i].isOppBlock == LinOfTiles[i + 1].isOppBlock) && LinOfTiles[i].isOppBlock);
            if (LinOfTiles[i].Number != 0 && LinOfTiles[i].Number == LinOfTiles[i + 1].Number
                && !LinOfTiles[i + 1].mergerThisTile && !LinOfTiles[i].mergerThisTile &&!canMerge)
            {
                LinOfTiles[i].isOppBlock = LinOfTiles[i + 1].isOppBlock;
                LinOfTiles[i].Number *= 2;
                if (pV.IsMine)
                    HudView.Instance.UpdateScore(LinOfTiles[i].Number, pV);
                LinOfTiles[i + 1].Number = 0;
                LinOfTiles[i].mergerThisTile = true;
                return true;
            }
        }
        return false;
    }
    bool MakeOneMoveUpIndex(Tile[] LinOfTiles)
    {
        // Debug.Log("the linoftiles:::MakeOneMoveupIndex::;" + LinOfTiles.Length);
        for (int i = LinOfTiles.Length - 1; i > 0; i--)
        {
            if (LinOfTiles[i].Number == 0 && LinOfTiles[i - 1].Number != 0)
            {
                LinOfTiles[i].isOppBlock = LinOfTiles[i - 1].isOppBlock;
                LinOfTiles[i].Number = LinOfTiles[i - 1].Number;
                LinOfTiles[i - 1].Number = 0;
                return true;
            }
            bool canMerge = ((LinOfTiles[i].isOppBlock == LinOfTiles[i - 1].isOppBlock) && LinOfTiles[i].isOppBlock);
            if (LinOfTiles[i].Number != 0 && LinOfTiles[i].Number == LinOfTiles[i - 1].Number
                 && !LinOfTiles[i - 1].mergerThisTile && !LinOfTiles[i].mergerThisTile && !canMerge)
            {
                LinOfTiles[i].isOppBlock = LinOfTiles[i - 1].isOppBlock;
                LinOfTiles[i].Number *= 2;
                if(pV.IsMine)
                HudView.Instance.UpdateScore(LinOfTiles[i].Number,pV);
                LinOfTiles[i - 1].Number = 0;
                LinOfTiles[i].mergerThisTile = true;
                return true;
            }
        }
        return false;
    }
    bool moveMade = false;
    public void Move(MoveDirection md, string tileData = "", int val = 0)
    {
        //if (moveMade)
        //    return;
        moveMade = false;
        ResetMergerFlags();
        for (int i = 0; i < rows.Count; i++)
        {
            switch (md)
            {
                case MoveDirection.Up:
                    while (MakeOneMoveDownIndex(columns[i]))
                    {
                        moveMade = true;
                    }
                    break;
                case MoveDirection.Down:
                    while (MakeOneMoveUpIndex(columns[i]))
                    {
                        moveMade = true;
                    }
                    break;
                case MoveDirection.Right:
                    while (MakeOneMoveUpIndex(rows[i]))
                    {
                        moveMade = true;
                    }
                    break;
                case MoveDirection.Left:
                    while (MakeOneMoveDownIndex(rows[i]))
                    {
                        moveMade = true;
                    }
                    break;
            }
        }
        //movemade used to generate new ones and avoid same movements
        if (moveMade)
        {
            moveMade = false;
            UpdateEmptyTiles();
            if (pV.IsMine)
            {
                data = string.Empty;
                Generate();
                mvDr = md;
                valueSent += 1;
                TileData dt = new TileData(pV.ViewID, mvDr, data, valueSent);
                sendingDataQueue.Enqueue(dt);
                Debug.Log("the data ha sent::" + mvDr + "value:;" + valueSent);
            }
            else if (!string.IsNullOrEmpty(tileData))
            {
                setTile(tileData);
            }
            if(!CanMove())
            {
                GameManager.Instance.EndOfGame();
            }
        }      
    }


    bool CanMove()
    {
        if (emptyTiles.Count > 0)
            return true;
        else
        {
            for (int i = 0; i < columns.Count; i++)
            {
                for (int j = 0; j < rows.Count - 1; j++)
                {
                    if (allTiles[j, i].Number == allTiles[j + 1, i].Number)
                    {
                        bool cannotMerge = ((allTiles[j, i].isOppBlock == allTiles[j + 1, i].isOppBlock) && allTiles[j, i].isOppBlock);
                        if (!cannotMerge)
                            return true;
                    }
                }
            }
            for (int i = 0; i < rows.Count; i++)
            {
                for (int j = 0; j < columns.Count - 1; j++)
                {
                    if (allTiles[i, j].Number == allTiles[i, j+1].Number)
                    {
                        bool cannotMerge = ((allTiles[i, j].isOppBlock == allTiles[i, j+1].isOppBlock) && allTiles[i, j].isOppBlock);
                        if (!cannotMerge)
                            return true;
                    }
                }
            }
        }
        return false;
    }
    int previousVal = 0;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (sendingDataQueue.Count > 0)
            {
                TileData storedData = sendingDataQueue.Peek();
                int Id = storedData.id;
                MoveDirection direction = storedData.movDirection;
                string tiledt = storedData.tileDt;
                int valuenum = storedData.valueNumber;
                bool isBlockedTile = storedData.generateBlock;
                string _data = Id.ToString() + "$" + direction.ToString() + "$" + tiledt + "$" + valuenum.ToString()+"$"+isBlockedTile;
               // Debug.Log("data" + _data);
                stream.SendNext(_data);
                sendingDataQueue.Dequeue();
            }
        }
        else
        {
            MoveDirection previousMove = mvDr;
            string receivedData = (string)stream.ReceiveNext();
           // Debug.Log("the data::" + receivedData+"id::"+pV.ViewID);

            string[] dt = receivedData.Split('$');
            int ID = int.Parse(dt[0]);
            bool isblocked = bool.Parse(dt[4]);
            if ((ID == pV.ViewID || isblocked))// ||(ID!=pV.ViewID && isblocked)
            {
               // Debug.LogFormat("the entered the bock with id {0} and pv id {1} bool value {2}:::", ID, pV.ViewID, isblocked);
                mvDr = (MoveDirection)System.Enum.Parse(typeof(MoveDirection), dt[1]);
                string tileData = dt[2];
                int _valueee = int.Parse(dt[3]);
                TileData dataStore = new TileData(ID, mvDr, tileData, _valueee,isblocked);
                dataQueue.Enqueue(dataStore);
            }
        }
    }

    void setTile(string _data)
    {
        string[] splitdata = _data.Split('|');
        // Debug.Log("the data:::" + int.Parse(splitdata[0]) + "::" + int.Parse(splitdata[1]) + "::" + int.Parse(splitdata[2]) + "::" + int.Parse(splitdata[3]));
        int index1 = int.Parse(splitdata[0]);
        int num1 = int.Parse(splitdata[1]);

        //Debug.Log("the index1:::" + index1 + ":::" + ":::" + emptyTiles.Count);
        emptyTiles[index1].Number = num1;
        emptyTiles.RemoveAt(index1);

    }

    public void TileButtonAction(Tile tile)
    {
            string data = tile.Row + "," + tile.Col + "$" + tile.Number;
            pV.RPC("RPC_TileAction", RpcTarget.AllBuffered, data); 
    }

    [PunRPC]
    void RPC_TileAction(string tileData)
    {
        string[] dt = tileData.Split('$');
        int num = int.Parse(dt[1]);
        string[] dt1 = dt[0].Split(',');
        int row = int.Parse(dt1[0]);
        int col = int.Parse(dt1[1]);
        Tile tile = allTiles[row, col];
        Debug.Log("the string data::" + dt[0] + ":::" + tile);
        if (num > 8 && GameManager.Instance.blockerTileObjToTakeFrom.childCount > 0)
        {
            tile.Number = 0;
            if (!emptyTiles.Contains(tile))
                emptyTiles.Add(tile);           
            GameObject blockerTile = GameManager.Instance.blockerTileObjToTakeFrom.GetChild(0).gameObject;
            Tile blockTile = blockerTile.GetComponent<Tile>();            
            blockTile.ResetValues();            
            if (!pV.IsMine)
            {
                blockTile.isOppBlock = true;
            }           
            blockTile.Number = num;
            blockerTile.transform.SetParent(tile.transform, false);
            GameObject sameNumberObj = GetPlaceWithSameNumberToRemove(blockTile);
          
          
            if (sameNumberObj)
            {
                if (sameNumberObj.transform.parent.GetSiblingIndex() == 0)
                    TimeCounter.Instance.timerRunning = false;         
                GameManager.Instance.blockersList.Remove(sameNumberObj.GetComponent<Tile>());
                blockerTile.transform.DOMove(sameNumberObj.transform.position, 1f).OnComplete(() => DestroyBlockerTile(blockTile, sameNumberObj));
            }
            else
            {
                GameManager.Instance.blockersList.Add(blockTile);
                int index = GameManager.Instance.blockersList.Count - 1;
                Transform placeToInstantiate = GameManager.Instance.blockerTilePanel.GetChild(index);
                blockerTile.transform.DOMove(placeToInstantiate.position, 1f).OnComplete(() => SetTileDataOnBlockerTile(blockerTile, placeToInstantiate));              
            }
        }
    }


  
    void DestroyBlockerTile(Tile BlockerTile, GameObject sameNumberObj)
    {  
        Transform parentBlock = sameNumberObj.transform.parent;

        BlockerTile.transform.SetParent(GameManager.Instance.blockerTileObjToTakeFrom);     
        sameNumberObj.transform.SetParent(GameManager.Instance.blockerTileObjToTakeFrom);
        
        if (parentBlock.GetSiblingIndex() == 0)
        {
            parentBlock.SetAsLastSibling();
            if(!TimeCounter.Instance.timerRunning)
            setTimerForFirstBlockerObject();
        }
        else
        {
            parentBlock.SetAsLastSibling();
        }    
    }

    GameObject GetPlaceWithSameNumberToRemove(Tile blockerTile)
    {
        GameObject placeToInstantiate = null;

        Tile selectedTile = GameManager.Instance.blockersList.Find(x => (x.Number == blockerTile.Number && x.isOppBlock != blockerTile.isOppBlock));
        if (selectedTile)
        {
            placeToInstantiate = selectedTile.gameObject;
            int index = GameManager.Instance.blockersList.IndexOf(selectedTile);
            isFirstIndex=(index == 0) ? true :  false ;           
        }

        return placeToInstantiate;
    }

    void SetTileDataOnBlockerTile(GameObject tileObj,Transform parent)
    {    
        tileObj.transform.SetParent(parent);
        if (parent.GetSiblingIndex()==0 && !TimeCounter.Instance.timerRunning)
        {
            setTimerForFirstBlockerObject();
        }      
    }

    void setTimerForFirstBlockerObject()
    {
        if (GameManager.Instance.blockersList.Count > 0)
        {            
            if (!PhotonNetwork.IsMasterClient)
                return;
            Hashtable prop = new Hashtable
            {
                {TimeCounter.timer,(float)PhotonNetwork.Time }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(prop);
        }
    }
  
    public void OnTimerCompleteAction(Tile blockTile)
    {
        if (pV.IsMine  && blockTile.isOppBlock && emptyTiles.Count > 0)
        {            
            blockTile.transform.DOMove(transform.position, 1f).OnComplete(() => ProvideTile(blockTile));
        }
        if (!pV.IsMine && !blockTile.isOppBlock)
        {
            blockTile.transform.DOMove(transform.position, 1f);
        }

    }

    void ProvideTile(Tile blockTile)
    {           
        int index = Random.Range(0, emptyTiles.Count - 1);
        string _data = index + "|" + blockTile.Number;
        valueSent += 1;
        TileData dt = new TileData(pV.ViewID, MoveDirection.None, _data, valueSent, true);
        sendingDataQueue.Enqueue(dt);
        GenerateAndRemoveBlockTileToSetTimer(_data,dt.id);
    } 

    void GenerateAndRemoveBlockTileToSetTimer(string _data,int id)
    {       
        string[] tileData = _data.Split('|');
        int index = int.Parse(tileData[0]);
        int number = int.Parse(tileData[1]);

            emptyTiles[index].isOppBlock = true;
        
            emptyTiles[index].Number = number;
            emptyTiles.RemoveAt(index);
       

        Tile blockTile = GameManager.Instance.blockersList[0];
        Transform parentBlock = blockTile.transform.parent;       
        blockTile.transform.SetParent(GameManager.Instance.blockerTileObjToTakeFrom);       
        GameManager.Instance.blockersList.RemoveAt(0);
        if (parentBlock.GetSiblingIndex() == 0 && !TimeCounter.Instance.timerRunning)
        {
            parentBlock.SetAsLastSibling();
            setTimerForFirstBlockerObject();
        }
        else
        {
            parentBlock.SetAsLastSibling();
        }       
    }
}
