using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Photon.Pun;

public class Tile : MonoBehaviour
{
    public int Number
    {
        get
        {
            return number;
        }
        set
        {
            number = value;
            if (number == 0)
                SetEmpty();
            else
            {
                SetVisible();
                GetIndexFromTileNumber(number);
            }
        }
    }
    int number;
    public Text tileText;
    public Image tileImage;
    public int Row;
    public int Col;
    public bool mergerThisTile = false;  
    public Image timerImage;
    public bool isOppBlock = false;
    public void ApplyTileHolder(int index,int number=0)
    {
        //Debug.Log("the color::" + index + ":::" + tileImage);
        TileStyle tileInfo = GameManager.Instance.tileStyleHolder.tiles[index];
        tileImage.color = tileInfo.tileColor;
        tileText.color = tileInfo.textColor;
        tileText.text = tileInfo.tileNumber.ToString();
        if (index==12)
        {
            tileText.text = number.ToString();
        }
             
    }

    void SetVisible()
    {
        tileImage.enabled = true;
        tileText.enabled = true;
    }
    void SetEmpty()
    {
        isOppBlock = false;
        mergerThisTile = false;
        tileImage.enabled = false;
        tileText.enabled = false;
    }

    void GetIndexFromTileNumber(int _number)
    {
        if(isOppBlock)
        {
            ApplyTileHolder(12,_number);
            return;
        }
        switch (_number)
        {
            case 2:
                ApplyTileHolder(0);
                break;
            case 4:
                ApplyTileHolder(1);
                break;
            case 8:
                ApplyTileHolder(2);
                break;
            case 16:
                ApplyTileHolder(3);
                break;
            case 32:
                ApplyTileHolder(4);
                break;
            case 64:
                ApplyTileHolder(5);
                break;
            case 128:
                ApplyTileHolder(6);
                break;
            case 256:
                ApplyTileHolder(7);
                break;
            case 512:
                ApplyTileHolder(8);
                break;
            case 1024:
                ApplyTileHolder(9);
                break;
            case 2048:
                ApplyTileHolder(10);
                break;
            case 4096:
                ApplyTileHolder(11);
                break;
            default:
                Debug.Log("Check the number");
                break;
        }
    }

    public void OnButtonAction()
    {
        GamePanel parentScript = transform.parent.GetComponent<GamePanel>();
        if (parentScript.pV.IsMine && !isOppBlock)
        {
            if (number > 8 && GameManager.Instance.blockerTileObjToTakeFrom.childCount > 0)
            {

                parentScript.TileButtonAction(this);
            }
        }
    }

    public void ResetValues()
    {
        transform.localPosition = Vector3.zero;      
        Number = 0;          
        Row = 0;
        Col = 0;
        timerImage.fillAmount = 1;
        timerImage.enabled = false;
        Debug.Log("the tile resetvalues is called:::" + this.gameObject + "::" + tileImage.fillAmount);
    }
}
